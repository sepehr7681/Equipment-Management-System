using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class EquipmentManagementSystem
{
    private List<Equipment> equipments = new List<Equipment>();
    private List<User> users = new List<User>();
    private List<Loan> loans = new List<Loan>();
    private List<Reservation> reservations = new List<Reservation>();

    public void AddUser(string id, string name, UserType type)
    {
        if (!id.StartsWith("USR-1204")) throw new Exception("User ID must start with USR-1204");
        if (users.Any(u => u.Id == id)) throw new Exception("User with ID already exists");

        users.Add(new User(id, name, type));
    }

    public void AddEquipment(string id, string name, string type)
    {
        if (!id.StartsWith("EQ-1204")) throw new Exception("Equipment ID must start with EQ-1204");
        if (equipments.Any(e => e.Id == id)) throw new Exception("Equipment with ID already exists");

        Equipment eq = type switch
        {
            "LAPTOP" => new Laptop(id, name),
            "TABLET" => new Tablet(id, name),
            "PROJECTOR" => new Projector(id, name),
            _ => throw new Exception("Invalid equipment type")
        };

        equipments.Add(eq);
    }

    public void BorrowEquipment(string userId, string equipId, DateTime date)
    {
        User u = users.FirstOrDefault(x => x.Id == userId) ?? throw new Exception("USER NOT FOUND");
        Equipment eq = equipments.FirstOrDefault(x => x.Id == equipId) ?? throw new Exception("EQUIPMENT NOT FOUND");

        if (!u.IsActive) throw new Exception("USER IS INACTIVE");
        if (eq.Status != EquipmentStatus.Available) throw new Exception("EQUIPMENT IS NOT AVAILABLE");
        if (!eq.CanBeBorrowedBy(u)) throw new Exception("BORROW RESTRICTED BY RULE B4");
        if (u.CurrentBorrowedCount >= u.GetMaxAllowedLoans()) throw new Exception("USER LOAN LIMIT REACHED");
        if (reservations.Any(r => r.Item.Id == eq.Id && r.Requestor.Id != u.Id && r.Status == ReservationStatus.Pending)) throw new Exception("EQUIPMENT IS RESERVED BY ANOTHER USER");

        eq.SetStatus(EquipmentStatus.Borrowed);
        eq.IncrementBorrowCount();
        u.CurrentBorrowedCount++;

        loans.Add(new Loan(u, eq, date));
        u.LoanHistory.Add($"Borrowed {eq.Name} on {date:yyyy-MM-dd}");
    }

    public double ReturnEquipment(string userId, string equipId, DateTime returnDate)
    {
        Loan loan = loans.FirstOrDefault(l => l.Borrower.Id == userId &&
                                              l.Item.Id == equipId &&
                                              l.Status == LoanStatus.Active)
                    ?? throw new Exception("NO ACTIVE LOAN FOUND FOR THIS USER AND EQUIPMENT");

        loan.ReturnDate = returnDate;
        loan.Status = LoanStatus.Returned;
        loan.FineAmount = loan.CalculateFine(returnDate);
        loan.Borrower.CurrentBorrowedCount--;

        if (loan.Item.BorrowCount < 7) loan.Item.SetStatus(EquipmentStatus.Available);

        return loan.FineAmount;
    }

    public void ReserveEquipment(string userId, string equipId, DateTime neededDate)
    {
        User u = users.FirstOrDefault(x => x.Id == userId) ?? throw new Exception("USER NOT FOUND");
        Equipment eq = equipments.FirstOrDefault(x => x.Id == equipId) ?? throw new Exception("EQUIPMENT NOT FOUND");

        if (reservations.Any(r => r.Requestor.Id == u.Id && r.Item.Id == eq.Id && r.Status == ReservationStatus.Pending)) throw new Exception("DUPLICATE RESERVATION");

        reservations.Add(new Reservation(u, eq, neededDate));
    }

    public List<Equipment> SearchEquipment(string keyword, string searchType) => searchType switch
    {
        "ID" => equipments.Where(e => e.Id == keyword).ToList(),
        "NAME" => equipments.Where(e => e.Name.Contains(keyword)).ToList(),
        "STATUS" => equipments.Where(e => e.Status.ToString() == keyword).ToList(),
        _ => new List<Equipment>()
    };

    public void GenerateReportC1(TextWriter outputWriter = null)
    {
        TextWriter writer = outputWriter ?? Console.Out;
        writer.WriteLine("===== REPORT C1: USAGE ANALYSIS (SORTED BY BORROW COUNT) =====");

        var sortedList = equipments.OrderByDescending(x => x.BorrowCount).ToList();
        if (!sortedList.Any())
        {
            writer.WriteLine("No equipment records found.");
            return;
        }

        writer.WriteLine("--- High Usage Equipment ---");
        foreach (var item in sortedList.Where(i => i.BorrowCount >= 3))
            writer.WriteLine(item.GetReportDetails());

        writer.WriteLine("--- Low Usage Equipment ---");
        foreach (var item in sortedList.Where(i => i.BorrowCount < 3))
            writer.WriteLine(item.GetReportDetails());

        writer.WriteLine("=============================================================");
    }

    public void ProcessCommand(string commandLine, TextWriter writer)
    {
        if (string.IsNullOrWhiteSpace(commandLine)) return;

        string[] parts = commandLine.Split(' ');
        string action = parts[0];

        try
        {
            switch (action)
            {
                case "ADD_USER":
                    AddUser(parts[1], parts[3], Enum.Parse<UserType>(parts[2], true));
                    writer.WriteLine($"SUCCESS: USER {parts[1]} ADDED");
                    break;

                case "ADD_EQUIPMENT":
                    AddEquipment(parts[1], parts[3], parts[2]);
                    writer.WriteLine($"SUCCESS: EQUIPMENT {parts[1]} ADDED");
                    break;

                case "BORROW":
                    BorrowEquipment(parts[1], parts[2], DateTime.Parse(parts[3]));
                    writer.WriteLine("SUCCESS: LOAN CREATED");
                    break;

                case "RETURN":
                    double fine = ReturnEquipment(parts[1], parts[2], DateTime.Parse(parts[3]));
                    writer.WriteLine($"SUCCESS: EQUIPMENT RETURNED. FINE: {fine}");
                    break;

                case "SEARCH":
                    var results = SearchEquipment(parts[2], parts[1]);
                    writer.WriteLine($"SUCCESS: FOUND {results.Count} ITEMS");
                    foreach (var res in results) writer.WriteLine($" - {res.GetReportDetails()}");
                    break;

                case "REPORT":
                    if (parts[1] == "C1") GenerateReportC1(writer);
                    break;

                default:
                    writer.WriteLine("ERROR: UNKNOWN COMMAND");
                    break;
            }
        }
        catch (Exception ex)
        {
            writer.WriteLine($"ERROR: {ex.Message.ToUpper()}");
        }
    }
}
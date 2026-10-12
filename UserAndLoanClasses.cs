using System;
using System.Collections.Generic;

public class User : IReportable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public UserType Type { get; set; }
    public bool IsActive { get; set; } = true;
    public int CurrentBorrowedCount { get; set; }
    public List<string> LoanHistory { get; set; } = new List<string>();

    public User(string id, string name, UserType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public int GetMaxAllowedLoans() => Type == UserType.Student ? 2 : 8;

    public string GetReportDetails() =>
        $"User [{Id}] - {Name} ({Type}) | Active Loans: {CurrentBorrowedCount}/{GetMaxAllowedLoans()} | Status: {(IsActive ? "Active" : "Disabled")}";
}

public class Loan
{
    public string LoanId { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
    public User Borrower { get; set; }
    public Equipment Item { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; } = LoanStatus.Active;
    public double FineAmount { get; set; }

    public Loan(User user, Equipment item, DateTime borrowDate)
    {
        Borrower = user;
        Item = item;
        BorrowDate = borrowDate;
        DueDate = borrowDate.AddDays(item.CalculateMaximumLoanDays());
    }

    public double CalculateFine(DateTime actualReturnDate) =>
        actualReturnDate <= DueDate ? 0 : (actualReturnDate - DueDate).Days * 50;
}

public class Reservation
{
    public string ReservationId { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
    public User Requestor { get; set; }
    public Equipment Item { get; set; }
    public DateTime RequestDate { get; set; } = DateTime.Now;
    public DateTime NeededDate { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    public Reservation(User user, Equipment item, DateTime neededDate)
    {
        Requestor = user;
        Item = item;
        NeededDate = neededDate;
    }
}
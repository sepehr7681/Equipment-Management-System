using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        EquipmentManagementSystem system = new EquipmentManagementSystem();

        if (File.Exists("input.txt"))
        {
            Console.WriteLine("Processing input.txt file...");
            using StreamReader reader = new StreamReader("input.txt");
            using StreamWriter writer = new StreamWriter("output.txt");

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                system.ProcessCommand(line, writer);
            }

            Console.WriteLine("Processing complete. Results saved to output.txt");
            return;
        }

        while (true)
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine(" UNIVERSITY EQUIPMENT MANAGEMENT SYSTEM ");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Add User\n2. Add Equipment\n3. Borrow Equipment\n4. Return Equipment");
            Console.WriteLine("5. Reserve Equipment\n6. Search Equipment\n7. Show Report C1");
            Console.WriteLine("8. Execute Single Command\n9. Exit");
            Console.Write("Select (1-9): ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("User ID: "); string uid = Console.ReadLine();
                        Console.Write("Name: "); string uname = Console.ReadLine();
                        Console.Write("Type (Student/Faculty): "); UserType utype = Enum.Parse<UserType>(Console.ReadLine());
                        system.AddUser(uid, uname, utype);
                        Console.WriteLine("SUCCESS: User added");
                        break;

                    case "2":
                        Console.Write("Equipment ID: "); string eid = Console.ReadLine();
                        Console.Write("Name: "); string ename = Console.ReadLine();
                        Console.Write("Type (LAPTOP/TABLET/PROJECTOR): "); string etype = Console.ReadLine();
                        system.AddEquipment(eid, ename, etype);
                        Console.WriteLine("SUCCESS: Equipment added");
                        break;

                    case "3":
                        Console.Write("User ID: "); string buid = Console.ReadLine();
                        Console.Write("Equipment ID: "); string beid = Console.ReadLine();
                        Console.Write("Date (YYYY-MM-DD): "); DateTime bdate = DateTime.Parse(Console.ReadLine());
                        system.BorrowEquipment(buid, beid, bdate);
                        Console.WriteLine("SUCCESS: Borrowed");
                        break;

                    case "4":
                        Console.Write("User ID: "); string ruid = Console.ReadLine();
                        Console.Write("Equipment ID: "); string reid = Console.ReadLine();
                        Console.Write("Return Date (YYYY-MM-DD): "); DateTime rdate = DateTime.Parse(Console.ReadLine());
                        double fine = system.ReturnEquipment(ruid, reid, rdate);
                        Console.WriteLine($"SUCCESS: Returned. Fine: {fine}");
                        break;

                    case "5":
                        Console.Write("User ID: "); string resUid = Console.ReadLine();
                        Console.Write("Equipment ID: "); string resEid = Console.ReadLine();
                        Console.Write("Needed Date (YYYY-MM-DD): "); DateTime resDate = DateTime.Parse(Console.ReadLine());
                        system.ReserveEquipment(resUid, resEid, resDate);
                        Console.WriteLine("SUCCESS: Reserved");
                        break;

                    case "6":
                        Console.Write("Search by (ID/NAME/STATUS): "); string stype = Console.ReadLine();
                        Console.Write("Keyword: "); string skey = Console.ReadLine();
                        var results = system.SearchEquipment(skey, stype);
                        Console.WriteLine($"Found {results.Count} items:");
                        foreach (var item in results) item.DisplayDetails();
                        break;

                    case "7":
                        system.GenerateReportC1();
                        break;

                    case "8":
                        Console.Write("Enter Command: ");
                        system.ProcessCommand(Console.ReadLine(), Console.Out);
                        break;

                    case "9":
                        Console.WriteLine("Bye!");
                        return;

                    default:
                        Console.WriteLine("ERROR: Invalid menu choice");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }
}
using System;

public abstract class Equipment : IBorrowable, IReportable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public EquipmentStatus Status { get; protected set; } = EquipmentStatus.Available;
    public int BorrowCount { get; protected set; }

    public Equipment(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public abstract int CalculateMaximumLoanDays();
    public abstract void DisplayDetails();

    public virtual bool CanBeBorrowedBy(User user) => Status == EquipmentStatus.Available;

    public void IncrementBorrowCount()
    {
        BorrowCount++;
        if (BorrowCount >= 7) Status = EquipmentStatus.UnderMaintenance;
    }

    public void SetStatus(EquipmentStatus status) => Status = status;

    public virtual string GetReportDetails() =>
        $"[ID: {Id}] {Name} | Type: {GetType().Name} | Status: {Status} | Times Borrowed: {BorrowCount}";
}

public class Laptop : Equipment
{
    public string RAM { get; set; }

    public Laptop(string id, string name, string ram = "16GB") : base(id, name) => RAM = ram;

    public override int CalculateMaximumLoanDays() => 9;

    public override void DisplayDetails() =>
        Console.WriteLine($"Laptop -> ID: {Id}, Name: {Name}, RAM: {RAM}, Status: {Status}, Borrowed: {BorrowCount} times");
}

public class Tablet : Equipment
{
    public bool HasStylus { get; set; }

    public Tablet(string id, string name, bool hasStylus = true) : base(id, name) => HasStylus = hasStylus;

    public override int CalculateMaximumLoanDays() => 9;

    public override void DisplayDetails() =>
        Console.WriteLine($"Tablet -> ID: {Id}, Name: {Name}, Stylus: {HasStylus}, Status: {Status}, Borrowed: {BorrowCount} times");
}

public class Projector : Equipment
{
    public string Resolution { get; set; }

    public Projector(string id, string name, string resolution = "4K") : base(id, name) => Resolution = resolution;

    public override int CalculateMaximumLoanDays() => 9;

    public override void DisplayDetails() =>
        Console.WriteLine($"Projector -> ID: {Id}, Name: {Name}, Resolution: {Resolution}, Status: {Status}, Borrowed: {BorrowCount} times");

    // Ghanoone B4: Faqat ostad ajazeye amanat gereftane projector ra darad
    public override bool CanBeBorrowedBy(User user) =>
        base.CanBeBorrowedBy(user) && user.Type == UserType.Faculty;
}
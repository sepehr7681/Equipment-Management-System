using System;

public enum EquipmentStatus
{
    Available,
    Borrowed,
    Reserved,
    UnderMaintenance,
    Disabled
}

public enum UserType
{
    Student,
    Faculty
}

public enum LoanStatus
{
    Active,
    Returned,
    Overdue
}

public enum ReservationStatus
{
    Pending,
    Fulfilled,
    Cancelled
}

public interface IReportable
{
    string GetReportDetails();
}

public interface IBorrowable
{
    bool CanBeBorrowedBy(User user);
}
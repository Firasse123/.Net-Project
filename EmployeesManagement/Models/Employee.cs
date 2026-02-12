using System;

namespace EmployeesManagement.Models
{
    public enum EmployeeStatus
    {
        Active,
        OnLeave,
        Terminated
    }

    public class Employee:UserActivity
    {

        public int Id   { get; set; }

        public string EmpNo  { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName  { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        public int PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Country { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }

        public string? ProfilePicture { get; set; }
        public EmployeeStatus Status { get; set; }

        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }
    }
}

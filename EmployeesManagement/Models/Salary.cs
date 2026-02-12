using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeesManagement.Models
{
    public enum SalaryStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Salary : UserActivity
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        public decimal BasicSalary { get; set; }

        public decimal? HousingAllowance { get; set; }

        public decimal? TransportAllowance { get; set; }

        public decimal? MedicalAllowance { get; set; }

        public decimal? OtherAllowances { get; set; }

        public decimal TotalSalary => BasicSalary + (HousingAllowance ?? 0) + 
                                       (TransportAllowance ?? 0) + (MedicalAllowance ?? 0) + 
                                       (OtherAllowances ?? 0);

        public DateTime EffectiveDate { get; set; }

        public string Currency { get; set; } = "USD";

        public string PaymentFrequency { get; set; } = "Monthly";

        public SalaryStatus Status { get; set; } = SalaryStatus.Pending;

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string? RejectionReason { get; set; }
    }
}

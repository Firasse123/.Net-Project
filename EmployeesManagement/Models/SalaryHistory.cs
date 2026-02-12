using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeesManagement.Models
{
    public class SalaryHistory : UserActivity
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        public decimal OldSalary { get; set; }

        [Required]
        public decimal NewSalary { get; set; }

        public decimal ChangeAmount => NewSalary - OldSalary;

        public decimal ChangePercentage => OldSalary > 0 ? ((NewSalary - OldSalary) / OldSalary) * 100 : 0;

        public DateTime EffectiveDate { get; set; }

        public string Reason { get; set; }

        public string Notes { get; set; }
    }
}

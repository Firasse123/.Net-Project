using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeesManagement.Models
{
    public enum BenefitType
    {
        HealthInsurance,
        LifeInsurance,
        PensionPlan,
        PaidTimeOff,
        Education,
        Gym,
        Other
    }

    public class Benefit : UserActivity
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        public string BenefitName { get; set; }

        public BenefitType Type { get; set; }

        public string Description { get; set; }

        public decimal? Value { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

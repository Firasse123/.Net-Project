using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeesManagement.Models
{
    public enum EquipmentType
    {
        Laptop,
        Desktop,
        Monitor,
        Phone,
        Tablet,
        Uniform,
        Vehicle,
        Furniture,
        Other
    }

    public enum EquipmentStatus
    {
        Available,
        Assigned,
        UnderMaintenance,
        Retired
    }

    public class Equipment : UserActivity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public EquipmentType Type { get; set; }

        public string SerialNumber { get; set; }

        public string Description { get; set; }

        public decimal? PurchasePrice { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public EquipmentStatus Status { get; set; }

        public int? AssignedToEmployeeId { get; set; }
        public Employee? AssignedToEmployee { get; set; }

        public DateTime? AssignmentDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public DateTime? WarrantyExpiry { get; set; }

        public string Notes { get; set; }
    }
}

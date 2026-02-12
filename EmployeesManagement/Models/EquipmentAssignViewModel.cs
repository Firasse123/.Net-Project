using System.ComponentModel.DataAnnotations;

namespace EmployeesManagement.Models
{
    public class EquipmentAssignViewModel
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; } = "";

        [Required]
        public int EmployeeId { get; set; }
    }
}

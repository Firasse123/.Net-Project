using System;
using System.Collections.Generic;

namespace EmployeesManagement.Models
{
    public class EquipmentAuditViewModel
    {
        public int TotalEquipment { get; set; }
        public int AssignedEquipment { get; set; }
        public int AvailableEquipment { get; set; }
        public int UnderMaintenanceEquipment { get; set; }
        public int RetiredEquipment { get; set; }
        public List<EquipmentTypeCount> ByType { get; set; } = new();
        public List<EmployeeAssignment> AssignmentsSummary { get; set; } = new();
    }

    public class EquipmentTypeCount
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public decimal Value { get; set; }
    }

    public class EmployeeAssignment
    {
        public string Employee { get; set; }
        public int Count { get; set; }
    }
}

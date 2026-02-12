using System;
using System.Collections.Generic;

namespace EmployeesManagement.Models
{
    public class BulkSalaryUpdateViewModel
    {
        public List<int> EmployeeIds { get; set; } = new List<int>();
        public decimal? PercentageIncrease { get; set; }
        public decimal? FlatIncrease { get; set; }
        public DateTime EffectiveDate { get; set; } = DateTime.Now;
        public string Reason { get; set; } = "Bulk Salary Update";
    }
}

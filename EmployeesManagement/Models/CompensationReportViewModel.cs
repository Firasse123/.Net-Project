using System.Collections.Generic;

namespace EmployeesManagement.Models
{
    public class CompensationReportViewModel
    {
        public decimal TotalPayroll { get; set; }
        public decimal AverageSalary { get; set; }
        public int EmployeesWithSalary { get; set; }
        public int TotalEmployees { get; set; }
        public List<DepartmentCompSummary> DepartmentSummaries { get; set; } = new List<DepartmentCompSummary>();
        public List<SalaryStatusSummary> StatusSummaries { get; set; } = new List<SalaryStatusSummary>();
    }

    public class DepartmentCompSummary
    {
        public string Department { get; set; } = "";
        public int EmployeeCount { get; set; }
        public decimal TotalPayroll { get; set; }
        public decimal AverageSalary { get; set; }
    }

    public class SalaryStatusSummary
    {
        public SalaryStatus Status { get; set; }
        public int Count { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace EmployeesManagement.Models
{
    public class RecruitmentStatsViewModel
    {
        public int TotalCandidates { get; set; }
        public int Applied { get; set; }
        public int UnderReview { get; set; }
        public int Interviewed { get; set; }
        public int Offered { get; set; }
        public int Hired { get; set; }
        public int Rejected { get; set; }
        public decimal HireRate { get; set; }
        public List<JobPositionStats> ByJobOpening { get; set; } = new();
    }

    public class JobPositionStats
    {
        public string Position { get; set; }
        public int Total { get; set; }
        public int Applied { get; set; }
        public int Interviewed { get; set; }
        public int Hired { get; set; }
        public int Rejected { get; set; }
    }
}

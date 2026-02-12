using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeesManagement.Models
{
    public enum JobStatus
    {
        Open,
        Closed,
        OnHold
    }

    public class JobOpening : UserActivity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string JobTitle { get; set; }

        [Required]
        [StringLength(100)]
        public string Department { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        [StringLength(2000)]
        public string Requirements { get; set; }

        [Range(1, 1000)]
        public int NumberOfPositions { get; set; }

        public DateTime PostedDate { get; set; }

        public DateTime? ClosingDate { get; set; }

        public JobStatus Status { get; set; }

        [StringLength(150)]
        public string Location { get; set; }

        [Range(0, 100000000)]
        public decimal? SalaryRangeMin { get; set; }

        [Range(0, 100000000)]
        public decimal? SalaryRangeMax { get; set; }

        // Navigation property
        public ICollection<Candidate>? Candidates { get; set; }
    }}
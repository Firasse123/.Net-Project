using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeesManagement.Models
{
    public enum CandidateStatus
    {
        Applied,
        UnderReview,
        Interviewed,
        Offered,
        Hired,
        Rejected
    }

    public class Candidate : UserActivity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; }

        [Phone]
        [StringLength(30)]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Resume { get; set; }

        public DateTime ApplicationDate { get; set; }

        public CandidateStatus Status { get; set; }

        [Required]
        public int JobOpeningId { get; set; }
        public JobOpening? JobOpening { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        public DateTime? InterviewDate { get; set; }

        [StringLength(2000)]
        public string? InterviewNotes { get; set; }
    }
}

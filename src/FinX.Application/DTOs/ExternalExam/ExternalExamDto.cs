using System;

namespace FinX.Application.DTOs.ExternalExam
{
    /// <summary>
    /// Data Transfer Object for external exam information
    /// </summary>
    public class ExternalExamDto
    {
        /// <summary>
        /// Unique identifier for the external exam
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Patient ID associated with this exam
        /// </summary>
        public Guid PatientId { get; set; }

        /// <summary>
        /// Type or name of the external exam
        /// </summary>
        public string ExamType { get; set; } = string.Empty;

        /// <summary>
        /// Date when the exam was performed
        /// </summary>
        public DateTime ExamDate { get; set; }

        /// <summary>
        /// Facility or clinic where the exam was performed
        /// </summary>
        public string Facility { get; set; } = string.Empty;

        /// <summary>
        /// Doctor who ordered or performed the exam
        /// </summary>
        public string Doctor { get; set; } = string.Empty;

        /// <summary>
        /// Results of the external exam
        /// </summary>
        public string Results { get; set; } = string.Empty;

        /// <summary>
        /// Additional notes or comments about the exam
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the exam record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the exam record was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
} 
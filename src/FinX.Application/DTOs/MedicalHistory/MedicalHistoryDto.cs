using System;

namespace FinX.Application.DTOs.MedicalHistory
{
    /// <summary>
    /// Data Transfer Object for medical history information
    /// </summary>
    public class MedicalHistoryDto
    {
        /// <summary>
        /// Unique identifier for the medical history record
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Patient ID associated with this medical history
        /// </summary>
        public Guid PatientId { get; set; }

        /// <summary>
        /// Type of medical history entry (e.g., Illness, Surgery, Medication)
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Description of the medical history item
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// When the medical condition or treatment started
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// When the medical condition or treatment ended (if applicable)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Additional notes or comments about the medical history
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the medical history record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the medical history record was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateMedicalHistoryDto
    {
        public Guid PatientId { get; set; }
        public string Diagnosis { get; set; }
        public string Exams { get; set; }
        public string Prescriptions { get; set; }
    }

    public class UpdateMedicalHistoryDto
    {
        public string Diagnosis { get; set; }
        public string Exams { get; set; }
        public string Prescriptions { get; set; }
    }
} 
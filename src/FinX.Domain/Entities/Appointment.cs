using System;

namespace FinX.Domain.Entities
{
    /// <summary>
    /// Represents a medical appointment in the system
    /// </summary>
    public class Appointment
    {
        /// <summary>
        /// Unique identifier for the appointment
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the patient associated with this appointment
        /// </summary>
        public Guid PatientId { get; set; }

        /// <summary>
        /// Reference to the patient entity
        /// </summary>
        public Patient? Patient { get; set; }

        /// <summary>
        /// Date and time of the appointment
        /// </summary>
        public DateTime AppointmentDate { get; set; }

        /// <summary>
        /// Duration of the appointment in minutes
        /// </summary>
        public int Duration { get; set; } = 30;

        /// <summary>
        /// Status of the appointment (e.g., Scheduled, Completed, Cancelled)
        /// </summary>
        public string Status { get; set; } = "Scheduled";

        /// <summary>
        /// Type of appointment (e.g., Initial Consultation, Follow-up, Examination)
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Additional notes or comments about the appointment
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the appointment record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the appointment record was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // Constructor for EF Core
        protected Appointment() { }

        public Appointment(
            Guid patientId,
            DateTime appointmentDate,
            int duration,
            string type,
            string notes = null)
        {
            Id = Guid.NewGuid();
            PatientId = patientId;
            AppointmentDate = appointmentDate;
            Duration = duration;
            Status = "Scheduled";
            Type = type;
            Notes = notes;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            DateTime appointmentDate,
            int duration,
            string status,
            string type,
            string notes)
        {
            AppointmentDate = appointmentDate;
            Duration = duration;
            Status = status;
            Type = type;
            Notes = notes;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 
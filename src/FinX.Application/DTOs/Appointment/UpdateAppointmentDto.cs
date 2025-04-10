using System;

namespace FinX.Application.DTOs.Appointment
{
    /// <summary>
    /// Data transfer object for updating an existing appointment
    /// </summary>
    public class UpdateAppointmentDto
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
        public string Status { get; set; } = string.Empty;
        
        /// <summary>
        /// Type of appointment (e.g., Initial Consultation, Follow-up, Examination)
        /// </summary>
        public string Type { get; set; } = string.Empty;
        
        /// <summary>
        /// Additional notes or comments about the appointment
        /// </summary>
        public string Notes { get; set; } = string.Empty;
    }
} 
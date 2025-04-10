using System;

namespace FinX.Application.DTOs.Appointment
{
    /// <summary>
    /// Data transfer object for creating a new appointment
    /// </summary>
    public class CreateAppointmentDto
    {
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
        /// Type of appointment (e.g., Initial Consultation, Follow-up, Examination)
        /// </summary>
        public string Type { get; set; } = string.Empty;
        
        /// <summary>
        /// Additional notes or comments about the appointment
        /// </summary>
        public string Notes { get; set; } = string.Empty;
    }
} 
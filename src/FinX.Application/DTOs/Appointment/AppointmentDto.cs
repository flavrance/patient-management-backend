using System;
using FinX.Application.DTOs.Patient;

namespace FinX.Application.DTOs.Appointment
{
    /// <summary>
    /// Data Transfer Object for Appointment information
    /// </summary>
    public class AppointmentDto
    {
        /// <summary>
        /// Appointment identifier
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Patient identifier
        /// </summary>
        public Guid PatientId { get; set; }
        
        /// <summary>
        /// Patient associated with the appointment
        /// </summary>
        public PatientDto Patient { get; set; }
        
        /// <summary>
        /// Date and time of the appointment
        /// </summary>
        public DateTime AppointmentDate { get; set; }
        
        /// <summary>
        /// Duration of the appointment in minutes
        /// </summary>
        public int Duration { get; set; }
        
        /// <summary>
        /// Status of the appointment
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Type of appointment
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Notes about the appointment
        /// </summary>
        public string Notes { get; set; }
        
        /// <summary>
        /// Date when the appointment was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Date when the appointment was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
} 
using System;

namespace FinX.Domain.Entities
{
    public class MedicalHistory
    {
        public Guid Id { get; private set; }
        public Guid PatientId { get; private set; }
        public string Diagnosis { get; private set; }
        public string Exams { get; private set; }
        public string Prescriptions { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public virtual Patient Patient { get; private set; }

        protected MedicalHistory() { }

        public MedicalHistory(Guid patientId, string diagnosis, string exams, string prescriptions)
        {
            Id = Guid.NewGuid();
            PatientId = patientId;
            Diagnosis = diagnosis;
            Exams = exams;
            Prescriptions = prescriptions;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(string diagnosis, string exams, string prescriptions)
        {
            Diagnosis = diagnosis;
            Exams = exams;
            Prescriptions = prescriptions;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 
using System;

namespace FinX.Domain.Entities
{
    public class ExternalExam
    {
        public Guid Id { get; private set; }
        public Guid PatientId { get; private set; }
        public string Name { get; private set; }
        public DateTime Date { get; private set; }
        public string Laboratory { get; private set; }
        public string Result { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public virtual Patient Patient { get; private set; }

        protected ExternalExam() { }

        public ExternalExam(
            Guid patientId,
            string name,
            DateTime date,
            string laboratory,
            string result)
        {
            Id = Guid.NewGuid();
            PatientId = patientId;
            Name = name;
            Date = date;
            Laboratory = laboratory;
            Result = result;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string name,
            DateTime date,
            string laboratory,
            string result)
        {
            Name = name;
            Date = date;
            Laboratory = laboratory;
            Result = result;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 
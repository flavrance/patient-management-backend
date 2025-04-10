using System;

namespace FinX.Application.DTOs.ExternalExams
{
    public class ExternalExamDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Laboratory { get; set; }
        public string Result { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateExternalExamDto
    {
        public Guid PatientId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Laboratory { get; set; }
        public string Result { get; set; }
    }

    public class UpdateExternalExamDto
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Laboratory { get; set; }
        public string Result { get; set; }
    }
} 
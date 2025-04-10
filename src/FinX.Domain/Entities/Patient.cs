using System;
using System.Collections.Generic;

namespace FinX.Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Cpf { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Gender { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Address { get; private set; }
        public string MedicalHistory { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public virtual ICollection<MedicalHistory> MedicalHistories { get; private set; }
        public virtual ICollection<ExternalExam> ExternalExams { get; private set; }
        public virtual ICollection<Appointment> Appointments { get; private set; }

        // Constructor for EF Core
        protected Patient() { }

        public Patient(
            string firstName,
            string lastName,
            string cpf,
            DateTime dateOfBirth,
            string gender,
            string email,
            string phoneNumber,
            string address,
            string medicalHistory = null)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Cpf = cpf;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            MedicalHistory = medicalHistory;
            CreatedAt = DateTime.UtcNow;
            MedicalHistories = new List<MedicalHistory>();
            ExternalExams = new List<ExternalExam>();
            Appointments = new List<Appointment>();
        }

        public void Update(
            string firstName,
            string lastName,
            string cpf,
            DateTime dateOfBirth,
            string gender,
            string email,
            string phoneNumber,
            string address,
            string medicalHistory)
        {
            FirstName = firstName;
            LastName = lastName;
            Cpf = cpf;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            MedicalHistory = medicalHistory;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 
using FinX.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinX.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<MedicalHistory> MedicalHistories { get; set; }
    public DbSet<ExternalExam> ExternalExams { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Cpf)
                .IsRequired()
                .HasMaxLength(11);
                
            entity.HasIndex(e => e.Cpf)
                .IsUnique();

            entity.Property(e => e.DateOfBirth)
                .IsRequired();

            entity.Property(e => e.Gender)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.MedicalHistory)
                .HasMaxLength(2000);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt);
        });

        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.PatientId)
                .IsRequired();
                
            entity.Property(e => e.Diagnosis)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.Exams)
                .IsRequired()
                .HasMaxLength(1000);
                
            entity.Property(e => e.Prescriptions)
                .IsRequired()
                .HasMaxLength(1000);
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.UpdatedAt);

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.MedicalHistories)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExternalExam>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.PatientId)
                .IsRequired();
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Date)
                .IsRequired();
                
            entity.Property(e => e.Laboratory)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Result)
                .IsRequired()
                .HasMaxLength(1000);
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.UpdatedAt);

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.ExternalExams)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.PatientId)
                .IsRequired();
                
            entity.Property(e => e.AppointmentDate)
                .IsRequired();
                
            entity.Property(e => e.Duration)
                .IsRequired();
                
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Notes)
                .HasMaxLength(1000);
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.UpdatedAt);

            entity.HasOne(e => e.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
} 
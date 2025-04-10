using System;

namespace FinX.Application.DTOs.Patient;

/// <summary>
/// DTO para representação de um paciente
/// </summary>
public class PatientDto
{
    /// <summary>
    /// Identificador único do paciente
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do paciente
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Sobrenome do paciente
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// CPF do paciente
    /// </summary>
    public string Cpf { get; set; }

    /// <summary>
    /// Data de nascimento do paciente
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gênero do paciente
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// Endereço de email do paciente
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Número de telefone do paciente
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Endereço do paciente
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Histórico médico do paciente
    /// </summary>
    public string MedicalHistory { get; set; }

    /// <summary>
    /// Data de criação do registro
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data da última atualização do registro
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
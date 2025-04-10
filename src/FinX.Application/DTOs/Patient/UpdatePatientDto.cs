using System;
using System.ComponentModel.DataAnnotations;

namespace FinX.Application.DTOs.Patient;

/// <summary>
/// DTO para atualização de um paciente existente
/// </summary>
public class UpdatePatientDto
{
    /// <summary>
    /// Nome do paciente
    /// </summary>
    [Required(ErrorMessage = "O nome é obrigatório")]
    [MaxLength(100, ErrorMessage = "O nome não pode ter mais que 100 caracteres")]
    public string FirstName { get; set; }

    /// <summary>
    /// Sobrenome do paciente
    /// </summary>
    [Required(ErrorMessage = "O sobrenome é obrigatório")]
    [MaxLength(100, ErrorMessage = "O sobrenome não pode ter mais que 100 caracteres")]
    public string LastName { get; set; }

    /// <summary>
    /// CPF do paciente
    /// </summary>
    [Required(ErrorMessage = "O CPF é obrigatório")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos")]
    public string Cpf { get; set; }

    /// <summary>
    /// Data de nascimento do paciente
    /// </summary>
    [Required(ErrorMessage = "A data de nascimento é obrigatória")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gênero do paciente
    /// </summary>
    [Required(ErrorMessage = "O gênero é obrigatório")]
    [MaxLength(20, ErrorMessage = "O gênero não pode ter mais que 20 caracteres")]
    public string Gender { get; set; }

    /// <summary>
    /// Endereço de email do paciente
    /// </summary>
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [MaxLength(100, ErrorMessage = "O email não pode ter mais que 100 caracteres")]
    public string Email { get; set; }

    /// <summary>
    /// Número de telefone do paciente
    /// </summary>
    [Required(ErrorMessage = "O telefone é obrigatório")]
    [MaxLength(20, ErrorMessage = "O telefone não pode ter mais que 20 caracteres")]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Endereço do paciente
    /// </summary>
    [Required(ErrorMessage = "O endereço é obrigatório")]
    [MaxLength(200, ErrorMessage = "O endereço não pode ter mais que 200 caracteres")]
    public string Address { get; set; }

    /// <summary>
    /// Histórico médico do paciente
    /// </summary>
    [MaxLength(2000, ErrorMessage = "O histórico médico não pode ter mais que 2000 caracteres")]
    public string MedicalHistory { get; set; }
} 
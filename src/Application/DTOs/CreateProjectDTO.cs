using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs;

public class CreateProjectDTO
{
    [Required(ErrorMessage = "Necessário informar o nome do projeto")]
    [JsonPropertyName("nome")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Necessário informar a descrição do projeto")]
    [JsonPropertyName("descricao")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Necessário informar o ID do usuário")]
    [JsonPropertyName("usuarioId")]
    public int UserId { get; set; }
}
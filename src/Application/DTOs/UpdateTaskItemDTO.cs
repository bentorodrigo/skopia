using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs;

public class UpdateTaskItemDTO
{
    [JsonPropertyName("status")]
    public Domain.Enums.TaskStatus Status { get; set; }

    [JsonPropertyName("descricao")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Necessário informar o ID do usuário")]
    [JsonPropertyName("usuarioId")]
    public int UserId { get; set; }
}

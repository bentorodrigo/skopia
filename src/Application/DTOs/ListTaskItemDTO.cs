using Domain.Enums;
using System.Text.Json.Serialization;

namespace Application.DTOs;

public class ListTaskItemDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("titulo")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("descricao")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("dataVencimento")]
    public DateTime DueDate { get; set; }

    [JsonPropertyName("status")]
    public Domain.Enums.TaskStatus Status { get; set; }

    [JsonPropertyName("prioridade")]
    public TaskPriority Priority { get; set; }

    [JsonPropertyName("projetoId")]
    public int ProjectId { get; set; }
}

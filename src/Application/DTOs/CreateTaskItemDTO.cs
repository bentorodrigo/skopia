using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs;

public class CreateTaskItemDTO
{
    [JsonPropertyName("titulo")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("descricao")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("dataVencimento")]
    public DateTime DueDate { get; set; }

    [Required(ErrorMessage = "Necessário informar a prioridade da tarefa")]
    [JsonPropertyName("prioridade")]
    public TaskPriority Priority { get; set; }

    [Required(ErrorMessage = "Necessário informar o ID do projeto")]
    [JsonPropertyName("projetoId")]
    public int ProjectId { get; set; }
}
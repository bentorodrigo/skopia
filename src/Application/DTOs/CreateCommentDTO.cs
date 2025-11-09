using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs;

public class CreateCommentDTO
{
    [Required(ErrorMessage = "Necessário informar o ID da tarefa")]
    [JsonPropertyName("tarefaId")]
    public int TaskItemId { get; set; }

    [JsonPropertyName("conteudo")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("autor")]
    public string Author { get; set; } = string.Empty;
}
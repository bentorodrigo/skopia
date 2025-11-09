using System.Text.Json.Serialization;

namespace Application.DTOs;

public class ListProjectDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("nome")]
    public string? Name { get; set; }

    [JsonPropertyName("descricao")]
    public string? Description { get; set; }

    [JsonPropertyName("usuarioId")]
    public int UserId { get; set; }
}
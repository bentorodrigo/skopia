using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

/// <summary>
/// Controlador responsável pelo gerenciamento de projetos dos usuários.
/// </summary>
[Route("api/projetos")]
[Produces("application/json")]
[SwaggerTag("Gerenciamento de projetos e listagem por usuário")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IProjectAppService _projectAppService;

    public ProjectController(IProjectAppService projectAppService)
    {
        _projectAppService = projectAppService;
    }

    /// <summary>
    /// Lista todos os projetos pertencentes a um usuário específico.
    /// </summary>
    /// <param name="usuarioId">Identificador único do usuário.</param>
    /// <returns>Uma lista de projetos ou uma mensagem informando que não foram encontrados projetos.</returns>
    /// <response code="200">Retorna a lista de projetos do usuário informado.</response>
    /// <response code="404">Nenhum projeto encontrado para o usuário informado.</response>
    /// <response code="400">Requisição inválida (por exemplo, ID ausente ou incorreto).</response>
    [SwaggerOperation(
        Summary = "Lista projetos de um usuário",
        Description = "Retorna todos os projetos associados a um usuário específico com base no ID informado."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Lista de projetos retornada com sucesso.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhum projeto encontrado para o usuário informado.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Parâmetro inválido ou ausente.")]
    [HttpGet("usuario/{usuarioId}")]
    public async Task<IActionResult> ListProjectsByUser(int usuarioId)
    {
        var projects = await _projectAppService.ListAllByUserAsync(usuarioId);
        if (projects != null && !projects.Data.Any())
            return NotFound($"Nenhum projeto encontrado para o usuario {usuarioId} informado");

        return Ok(projects.Data);
    }

    /// <summary>
    /// Cria um novo projeto vinculado a um usuário existente.
    /// </summary>
    /// <param name="createProjectDTO">Objeto contendo as informações do novo projeto.</param>
    /// <returns>O projeto criado e o link para listagem dos projetos do usuário.</returns>
    /// <response code="201">Projeto criado com sucesso.</response>
    /// <response code="400">Dados inválidos ou faltando informações obrigatórias.</response>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateProjectDTO createProjectDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _projectAppService.AddProjectAsync(createProjectDTO);
        return CreatedAtAction(nameof(ListProjectsByUser), new { userId = createProjectDTO.UserId }, createProjectDTO);
    }
}
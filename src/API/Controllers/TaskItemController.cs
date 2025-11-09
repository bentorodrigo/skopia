using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

/// <summary>
/// Controlador responsável pelo gerenciamento de tarefas de projetos.
/// </summary>
[Route("api/tarefas")]
[ApiController]
[Produces("application/json")]
[SwaggerTag("Gerenciamento de tarefas e comentários dentro dos projetos")]
public class TaskItemController : ControllerBase
{
    private readonly ITaskItemAppService _taskItemAppService;

    /// <summary>
    /// Inicializa uma nova instância do controlador de tarefas.
    /// </summary>
    /// <param name="taskItemAppService">Serviço de aplicação responsável pela lógica de tarefas.</param>
    public TaskItemController(ITaskItemAppService taskItemAppService)
    {
        _taskItemAppService = taskItemAppService;
    }

    /// <summary>
    /// Lista todas as tarefas pertencentes a um projeto específico.
    /// </summary>
    /// <param name="projetoId">Identificador único do projeto.</param>
    /// <returns>Uma lista de tarefas vinculadas ao projeto informado.</returns>
    /// <response code="200">Lista de tarefas retornada com sucesso.</response>
    /// <response code="404">Nenhuma tarefa encontrada para o projeto informado.</response>
    /// <response code="400">Requisição inválida (por exemplo, ID incorreto).</response>
    [HttpGet("projeto/{projetoId}")]
    [SwaggerOperation(
        Summary = "Lista tarefas de um projeto",
        Description = "Retorna todas as tarefas associadas a um projeto com base no ID informado."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Lista de tarefas retornada com sucesso.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhuma tarefa encontrada para o projeto informado.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Parâmetro inválido ou ausente.")]
    public async Task<IActionResult> ListTaskItemsByProject(int projetoId)
    {
        if (projetoId <= 0)
            return BadRequest("O ID do projeto deve ser maior que zero.");

        var taskItems = await _taskItemAppService.ListAllByProjectAsync(projetoId);

        if (taskItems == null || !taskItems.Data.Any())
            return NotFound($"Nenhuma tarefa encontrada para o projeto {projetoId} informado.");

        return Ok(taskItems.Data);
    }

    /// <summary>
    /// Cria uma nova tarefa associada a um projeto.
    /// </summary>
    /// <param name="createTaskItemDTO">Objeto contendo as informações da nova tarefa.</param>
    /// <returns>O objeto da tarefa criada.</returns>
    /// <response code="201">Tarefa criada com sucesso.</response>
    /// <response code="400">Dados inválidos ou faltando informações obrigatórias.</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cria uma nova tarefa",
        Description = "Adiciona uma nova tarefa a um projeto existente, respeitando as regras de limite de tarefas e prioridade."
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Tarefa criada com sucesso.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Os dados informados são inválidos.")]
    public async Task<IActionResult> Post([FromBody] CreateTaskItemDTO createTaskItemDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _taskItemAppService.AddTaskItemAsync(createTaskItemDTO);

        return CreatedAtAction(nameof(ListTaskItemsByProject),
            new { projectId = createTaskItemDTO.ProjectId },
            result.Data);
    }

    /// <summary>
    /// Adiciona um comentário a uma tarefa existente.
    /// </summary>
    /// <param name="createCommentDTO">Objeto contendo o comentário e o ID da tarefa.</param>
    /// <returns>Confirmação de criação do comentário.</returns>
    /// <response code="201">Comentário adicionado com sucesso.</response>
    /// <response code="400">Dados inválidos ou tarefa inexistente.</response>
    [HttpPost("comments")]
    [SwaggerOperation(
        Summary = "Adiciona um comentário a uma tarefa",
        Description = "Permite que um usuário adicione um comentário a uma tarefa existente. O comentário também é registrado no histórico de alterações da tarefa."
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Comentário adicionado com sucesso.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Os dados informados são inválidos.")]
    public async Task<IActionResult> PostComment([FromBody] CreateCommentDTO createCommentDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _taskItemAppService.AddCommentAsync(createCommentDTO);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Created();
    }

    /// <summary>
    /// Atualiza os detalhes ou o status de uma tarefa existente.
    /// </summary>
    /// <param name="id">Identificador único da tarefa.</param>
    /// <param name="updateTaskItemDTO">Objeto contendo os dados a serem atualizados.</param>
    /// <returns>Tarefa atualizada.</returns>
    /// <response code="200">Tarefa atualizada com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="404">Tarefa não encontrada.</response>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza uma tarefa existente",
        Description = "Permite atualizar o status, descrição ou outras informações da tarefa. Cada modificação é registrada no histórico de alterações."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Tarefa atualizada com sucesso.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Os dados informados são inválidos.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Tarefa não encontrada.")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateTaskItemDTO updateTaskItemDTO)
    {
        if (id <= 0)
            return BadRequest("O ID da tarefa deve ser maior que zero.");

        var result = await _taskItemAppService.UpdateTaskItemAsync(id, updateTaskItemDTO);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok();
    }

    /// <summary>
    /// Remove uma tarefa de um projeto.
    /// </summary>
    /// <param name="id">Identificador único da tarefa.</param>
    /// <returns>Confirmação da remoção.</returns>
    /// <response code="200">Tarefa removida com sucesso.</response>
    /// <response code="404">Tarefa não encontrada.</response>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Remove uma tarefa",
        Description = "Remove uma tarefa de um projeto existente, respeitando as regras de negócio relacionadas à exclusão de tarefas e projetos."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Tarefa removida com sucesso.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Tarefa não encontrada.")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest("O ID da tarefa deve ser maior que zero.");

        var result = await _taskItemAppService.DeleteTaskItemAsync(id);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok();
    }
}
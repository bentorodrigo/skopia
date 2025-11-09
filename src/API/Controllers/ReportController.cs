using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

/// <summary>
/// Controlador responsável pela geração de relatórios
/// </summary>
[Route("api/relatorios")]
[Produces("application/json")]
[SwaggerTag("Gerador de relatórios")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly IReportAppService _reportAppService;

    public ReportController(IReportAppService reportAppService)
    {
        _reportAppService = reportAppService;
    }

    /// <summary>
    /// Gera um relatorio de quantas tarefas foram concluídas pelo usuário informado
    /// </summary>
    /// <param name="usuarioId">Identificador único do usuário.</param>
    /// <returns>Um relatorio de desempenho sobre o usuário informado.</returns>
    /// <response code="200">Retorna os valores do relatorio a partir do usuário informado.</response>
    /// <response code="404">Nenhum usuario encontrado ou o usuário não possui a função de gerente.</response>
    [SwaggerOperation(
        Summary = "Relatorio de desempenho por usuário",
        Description = "Gera um relatorio de quantas tarefas foram concluídas com base no ID do usuário informado."
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Relatorio de desempenho sobre o usuário informado.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhum usuario encontrado ou o usuário não possui a função de gerente.")]
    [HttpGet("desempenho/usuario/{usuarioId}")]
    public async Task<IActionResult> Get(int usuarioId)
    {
        var report = await _reportAppService.GetReportsByUser(usuarioId);

        if (report != null && !report.Success)
            return NotFound(report.ErrorMessage);

        return Ok(report?.Data);
    }
}
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Domain.Services;

public class ReportService : IReportService
{
    private readonly IUserRepository _userRepository;
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<ReportService> _logger;

    public ReportService(IUserRepository userRepository,
                         IReportRepository reportRepository,
                         ILogger<ReportService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
        _reportRepository = reportRepository;
    }

    public async Task<Response<object>> GetReportsByUser(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            var errorMessage = "Não foi possível encontrar o usuário informado para a geração do relatório";
            _logger.LogInformation(errorMessage);
            return new Response<object>(new(), errorMessage);
        }

        if (user.Data != null && !user.Data.Function.Equals(UserFunction.Gerente))
        {
            var errorMessage = "Não é possível gerar relatórios, uma vez que o usuário não ter função de gerente";
            _logger.LogInformation(errorMessage);
            return new Response<object>(new(), errorMessage);
        }

        return new Response<object>(await _reportRepository.GetReportsByUser(userId));
    }
}

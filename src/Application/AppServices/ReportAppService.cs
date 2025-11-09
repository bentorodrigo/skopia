using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces.Services;

namespace Application.AppServices;

public class ReportAppService : IReportAppService
{
    private readonly IReportService _reportService;

    public ReportAppService(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<Response<object>> GetReportsByUser(int userId)
    {
        return await _reportService.GetReportsByUser(userId);
    }
}
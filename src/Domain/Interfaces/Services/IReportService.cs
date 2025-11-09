using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface IReportService
{
    public Task<Response<object>> GetReportsByUser(int userId);
}
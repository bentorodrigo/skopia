using Domain.Entities;

namespace Application.Interfaces;

public interface IReportAppService
{
    public Task<Response<object>> GetReportsByUser(int userId);
}
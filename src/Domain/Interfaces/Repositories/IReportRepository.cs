using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IReportRepository
{
    public Task<Response<object>> GetReportsByUser(int userId);
}
using Application.AppServices;
using Application.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Services;
using Infrastructure.Db;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection RegisterAppServices(this IServiceCollection serviceCollection) 
    {
        serviceCollection.AddScoped<IProjectAppService, ProjectAppService>();
        serviceCollection.AddScoped<ITaskItemAppService, TaskItemAppService>();
        serviceCollection.AddScoped<IReportAppService, ReportAppService>();
        return serviceCollection;
    }

    public static IServiceCollection RegisterDomainServices(this IServiceCollection serviceCollection) 
    {
        serviceCollection.AddScoped<IProjectService, ProjectService>();
        serviceCollection.AddScoped<ITaskItemService, TaskItemService>();
        serviceCollection.AddScoped<IReportService, ReportService>();
        return serviceCollection;
    }

    public static IServiceCollection ConfigureDatabase(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("SkopiaDB"));
        return serviceCollection;
    }

    public static IServiceCollection RegisterRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IProjectRepository, ProjectRepository>();
        serviceCollection.AddScoped<ITaskItemRepository, TaskItemRepository>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IReportRepository, ReportRepository>();
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        return serviceCollection;
    }
}
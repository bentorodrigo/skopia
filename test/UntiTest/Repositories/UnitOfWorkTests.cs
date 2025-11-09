using Domain.Entities;
using FluentAssertions;
using Infrastructure.Db;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.Repositories;

public class UnitOfWorkTests
{
    private readonly AppDbContext _dbContext;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
    }

    [Fact]
    public async Task CommitAsync_DeveRetornarTrue_QuandoHouverAlteracoes()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_dbContext);
        var user = new User { Name = "Rodrigo Bento", Email = "rodrigo@example.com" };
        _dbContext.Users.Add(user);

        // Act
        var result = await unitOfWork.CommitAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CommitAsync_DeveRetornarFalse_QuandoNaoHouverAlteracoes()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(_dbContext);

        // Act
        var result = await unitOfWork.CommitAsync();

        // Assert
        result.Should().BeFalse();
    }
}

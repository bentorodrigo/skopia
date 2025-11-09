using Domain.Entities;
using FluentAssertions;
using Infrastructure.Db;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.Repositories;

public class UserRepositoryTests
{
    private readonly AppDbContext _dbContext;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarUsuario_QuandoExistir()
    {
        // Arrange
        var user = new User { Id = 1, Name = "Rodrigo Bento", Email = "rodrigo@example.com" };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var repository = new UserRepository(_dbContext);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(1);
        result.Data!.Name.Should().Be("Rodrigo Bento");
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarMensagemErro_QuandoUsuarioNaoExistir()
    {
        // Arrange
        var repository = new UserRepository(_dbContext);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeNull();
    }
}

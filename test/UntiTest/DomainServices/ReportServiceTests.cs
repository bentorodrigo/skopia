using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.DomainServices;

public class ReportServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IReportRepository> _reportRepositoryMock;
    private readonly Mock<ILogger<ReportService>> _loggerMock;
    private readonly ReportService _service;

    public ReportServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _reportRepositoryMock = new Mock<IReportRepository>();
        _loggerMock = new Mock<ILogger<ReportService>>();

        _service = new ReportService(
            _userRepositoryMock.Object,
            _reportRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetReportsByUser_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Response<User>)null);

        // Act
        var result = await _service.GetReportsByUser(1);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Não foi possível encontrar o usuário");

        _reportRepositoryMock.Verify(x => x.GetReportsByUser(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetReportsByUser_ShouldReturnError_WhenUserIsNotManager()
    {
        // Arrange
        var user = new User { Id = 1, Function = UserFunction.Normal };
        var userResponse = new Response<User>(user);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(user.Id))
            .ReturnsAsync(userResponse);

        // Act
        var result = await _service.GetReportsByUser(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("não ter função de gerente");

        _reportRepositoryMock.Verify(x => x.GetReportsByUser(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetReportsByUser_ShouldReturnSuccess_WhenUserIsManager()
    {
        // Arrange
        var user = new User { Id = 1, Function = UserFunction.Gerente };
        var userResponse = new Response<User>(user);

        var reportData = new { TotalTarefas = 12, Concluidas = 10 };
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(user.Id))
            .ReturnsAsync(userResponse);

        _reportRepositoryMock
            .Setup(x => x.GetReportsByUser(user.Id))
            .ReturnsAsync(new Response<object>(reportData));

        // Act
        var result = await _service.GetReportsByUser(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNullOrEmpty();

        _userRepositoryMock.Verify(x => x.GetByIdAsync(user.Id), Times.Once);
        _reportRepositoryMock.Verify(x => x.GetReportsByUser(user.Id), Times.Once);
        _loggerMock.VerifyNoOtherCalls();
    }
}

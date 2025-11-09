using Application.AppServices;
using Domain.Entities;
using Domain.Interfaces.Services;
using FluentAssertions;
using Moq;

namespace UnitTest.AppServices;

public class ReportAppServiceTests
{
    private readonly Mock<IReportService> _mockReportService;
    private readonly ReportAppService _appService;

    public ReportAppServiceTests()
    {
        _mockReportService = new Mock<IReportService>();
        _appService = new ReportAppService(_mockReportService.Object);
    }

    [Fact]
    public async Task GetReportsByUser_ShouldReturnReport_WhenServiceSucceeds()
    {
        // Arrange
        var userId = 1;
        var reportData = new { CompletedTasks = 15, PendingTasks = 5 };
        var expectedResponse = new Response<object>(reportData);

        _mockReportService
            .Setup(s => s.GetReportsByUser(userId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _appService.GetReportsByUser(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(reportData);

        _mockReportService.Verify(s => s.GetReportsByUser(userId), Times.Once);
    }

    [Fact]
    public async Task GetReportsByUser_ShouldReturnError_WhenServiceFails()
    {
        // Arrange
        var userId = 2;
        var expectedResponse = new Response<object>(null, "Usuário não encontrado");

        _mockReportService
            .Setup(s => s.GetReportsByUser(userId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _appService.GetReportsByUser(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Usuário não encontrado");
        result.Data.Should().BeNull();

        _mockReportService.Verify(s => s.GetReportsByUser(userId), Times.Once);
    }
}
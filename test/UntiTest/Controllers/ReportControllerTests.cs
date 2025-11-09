using API.Controllers;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controllers;

public class ReportControllerTests
{
    private readonly Mock<IReportAppService> _mockReportAppService;
    private readonly ReportController _controller;

    public ReportControllerTests()
    {
        _mockReportAppService = new Mock<IReportAppService>();
        _controller = new ReportController(_mockReportAppService.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WhenReportIsSuccessful()
    {
        // Arrange
        var usuarioId = 1;
        var reportData = new { TarefasConcluidas = 5 };
        var response = new Response<object>(reportData);

        _mockReportAppService
            .Setup(s => s.GetReportsByUser(usuarioId))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Get(usuarioId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(reportData);

        _mockReportAppService.Verify(s => s.GetReportsByUser(usuarioId), Times.Once);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenReportFails()
    {
        // Arrange
        var usuarioId = 2;
        var response = new Response<object>(null, "Usuário não encontrado");

        _mockReportAppService
            .Setup(s => s.GetReportsByUser(usuarioId))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Get(usuarioId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Usuário não encontrado");

        _mockReportAppService.Verify(s => s.GetReportsByUser(usuarioId), Times.Once);
    }
}
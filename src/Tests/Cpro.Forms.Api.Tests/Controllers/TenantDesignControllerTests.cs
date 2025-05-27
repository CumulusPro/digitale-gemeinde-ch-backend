using Cpro.Forms.Api.Controllers;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Cpro.Forms.Api.Tests.Controllers;

public class TenantDesignControllerTests
{
    private readonly Mock<ITenantDesignService> _tenantDesignServiceMock;
    private readonly TenantDesignController _controller;

    public TenantDesignControllerTests()
    {
        _tenantDesignServiceMock = new Mock<ITenantDesignService>();
        _controller = new TenantDesignController(_tenantDesignServiceMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WhenDesignExists()
    {
        // Arrange
        var tenantId = 1;
        var expectedDesign = new TenantDesign();
        _tenantDesignServiceMock.Setup(x => x.GetTenantDesign(tenantId))
            .ReturnsAsync(expectedDesign);

        // Act
        var result = await _controller.Get(tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<TenantDesign>(okResult.Value);
        Assert.Equal(expectedDesign, returnValue);
    }

    [Fact]
    public async Task Post_ReturnsOkResult_WhenDesignCreated()
    {
        // Arrange
        var tenantId = 1;
        var design = new TenantDesign();
        var expectedDesign = new TenantDesign();
        _tenantDesignServiceMock.Setup(x => x.CreateUpdateTenantDesign(design, tenantId))
            .ReturnsAsync(expectedDesign);

        // Act
        var result = await _controller.Post(design, tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<TenantDesign>(okResult.Value);
        Assert.Equal(expectedDesign, returnValue);
    }
} 
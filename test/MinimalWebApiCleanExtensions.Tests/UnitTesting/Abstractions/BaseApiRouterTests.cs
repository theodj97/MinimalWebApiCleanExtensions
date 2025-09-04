using System.Reflection;
using MinimalWebApiCleanExtensions.Abstractions;
using static MinimalWebApiCleanExtensions.Tests.Shared.Mocks.MockRoutes.AssemblyRoutes;

namespace MinimalWebApiCleanExtensions.Tests.UnitTesting.Abstractions;

public class BaseApiRouterTests
{
    [Fact]
    public void Constructor_WithValidRouteNameEndingInRoutes_SetsRouteName()
    {
        // Arrange
        var contextAccessor = CreateTestContextAccessor();

        // Act
        var router = (BaseApiRouter)Activator.CreateInstance(typeof(TestApiRoutes), contextAccessor)!;

        // Assert
        Assert.Equal("TestApi", router.RouteName);
    }

    [Fact]
    public void Constructor_WithInvalidRouteNameEnding_ThrowsArgumentException()
    {
        // Arrange
        var contextAccessor = CreateTestContextAccessor();

        // Act & Assert
        var ex = Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(typeof(InvalidNameRouter), contextAccessor));
        Assert.IsType<ArgumentException>(ex.InnerException);
        Assert.Contains("Route name doesn't match syntax finishing in Routes", ex.InnerException.Message);
    }

    [Fact]
    public void UserName_Property_ReturnsCorrectClaimValue()
    {
        // Arrange
        var contextAccessor = CreateTestContextAccessor();
        var router = (BaseApiRouter)Activator.CreateInstance(typeof(TestApiRoutes), contextAccessor)!;

        // Act
        var userName = router.UserName;

        // Assert
        Assert.Equal("TestUser", userName);
    }

    [Fact]
    public void UserEmail_Property_ReturnsCorrectClaimValue()
    {
        // Arrange
        var contextAccessor = CreateTestContextAccessor();
        var router = (BaseApiRouter)Activator.CreateInstance(typeof(TestApiRoutes), contextAccessor)!;

        // Act
        var userEmail = router.UserEmail;

        // Assert
        Assert.Equal("test@example.com", userEmail);
    }

    [Fact]
    public void UserRole_Property_ReturnsCorrectClaimValue()
    {
        // Arrange
        var contextAccessor = CreateTestContextAccessor();
        var router = (BaseApiRouter)Activator.CreateInstance(typeof(TestApiRoutes), contextAccessor)!;

        // Act
        var userRole = router.UserRole;

        // Assert
        Assert.Equal("Admin", userRole);
    }
}
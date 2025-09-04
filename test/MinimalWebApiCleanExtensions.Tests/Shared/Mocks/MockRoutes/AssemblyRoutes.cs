using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalWebApiCleanExtensions.Abstractions;
using MinimalWebApiCleanExtensions.Interfaces;

namespace MinimalWebApiCleanExtensions.Tests.Shared.Mocks.MockRoutes;

public class AssemblyRoutes
{
    // Test implementation of BaseApiRouter with proper naming convention
    public class TestApiRoutes(IHttpContextAccessor contextAccessor) : BaseApiRouter(contextAccessor)
    {

        public override void MapGroup(IEndpointRouteBuilder app)
        {
            // Implementation not needed for testing properties
        }
    }

    // Invalid router for testing constructor validation (doesn't follow naming convention)
    public class InvalidNameRouter(IHttpContextAccessor contextAccessor) : BaseApiRouter(contextAccessor)
    {

        public override void MapGroup(IEndpointRouteBuilder app)
        {
            // Implementation not needed for testing
        }
    }

    // Class implementing IGroupMap - should be registered
    public class SimpleTestGroupMap : IGroupMap
    {
        public static bool WasMapGroupCalled { get; private set; } = false;

        public void MapGroup(IEndpointRouteBuilder app)
        {
            WasMapGroupCalled = true;
        }
    }

    // Abstract class implementing IGroupMap - should not be registered
    public abstract class AbstractGroupMap : IGroupMap
    {
        public abstract void MapGroup(IEndpointRouteBuilder app);
    }

    // Interface implementing IGroupMap - should not be registered
    public interface IInterfaceGroupMap : IGroupMap
    {
    }

    // Simple implementation of IHttpContextAccessor for testing
    public class TestHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; }
    }

    // Helper method to create a test context accessor with claims
    public static TestHttpContextAccessor CreateTestContextAccessor()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "TestUser"),
            new(ClaimTypes.Email, "test@example.com"),
            new(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        var contextAccessor = new TestHttpContextAccessor
        {
            HttpContext = httpContext
        };

        return contextAccessor;
    }
}

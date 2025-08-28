using Microsoft.AspNetCore.Routing;

namespace MinimalWebApiCleanExtensions.Interfaces;

public interface IGroupMap
{
    void MapGroup(IEndpointRouteBuilder app);
}

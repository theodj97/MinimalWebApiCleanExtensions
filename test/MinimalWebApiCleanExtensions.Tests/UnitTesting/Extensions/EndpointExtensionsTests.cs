using Microsoft.Extensions.DependencyInjection;
using MinimalWebApiCleanExtensions.Extensions;
using MinimalWebApiCleanExtensions.Interfaces;
using static MinimalWebApiCleanExtensions.Tests.Shared.Mocks.MockRoutes.AssemblyRoutes;

namespace MinimalWebApiCleanExtensions.Tests.UnitTesting.Extensions;

public class EndpointExtensionsTests
{
    [Fact]
    public void RegisterMinimalEndpoints_WithValidAssembly_RegistersGroupMapTypes()
    {
        // Arrange
        var servicesWithRegisteredEndpoints = new ServiceCollection();
        var servicesWithoutRegisteredEndpoints = new ServiceCollection();
        var assembly = typeof(EndpointExtensionsTests).Assembly;

        // Act
        var result = servicesWithRegisteredEndpoints.RegisterMinimalEndpoints(assembly);

        // Assert
        Assert.Same(servicesWithRegisteredEndpoints,
                    result);
        Assert.Contains(result, sd =>
            sd.ServiceType == typeof(IGroupMap)
            && sd.ImplementationType == typeof(SimpleTestGroupMap)
            && sd.Lifetime == ServiceLifetime.Transient
        );
        Assert.DoesNotContain(new ServiceDescriptor(typeof(IGroupMap), typeof(SimpleTestGroupMap), ServiceLifetime.Transient),
                              servicesWithoutRegisteredEndpoints);
        Assert.NotEqual(servicesWithRegisteredEndpoints.Count,
                        servicesWithoutRegisteredEndpoints.Count);
        Assert.DoesNotContain(new ServiceDescriptor(typeof(IGroupMap), typeof(SimpleTestGroupMap), ServiceLifetime.Transient),
              servicesWithoutRegisteredEndpoints);
    }

    [Fact]
    public void RegisterMinimalEndpoints_WithAssemblyContainingSimpleGroupMapTypes_RegistersThem()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(SimpleTestGroupMap).Assembly;

        // Act
        var result = services.RegisterMinimalEndpoints(assembly);

        // Assert
        Assert.Same(services, result);

        var descriptors = services.Where(sd => sd.ServiceType == typeof(IGroupMap)).ToList();
        Assert.NotEmpty(descriptors);
        Assert.True(descriptors.All(d => d.Lifetime == ServiceLifetime.Transient));
        Assert.Contains(descriptors, d => d.ImplementationType == typeof(SimpleTestGroupMap));
    }

    [Fact]
    public void RegisterMinimalEndpoints_DoesNotRegisterAbstractOrInterfaceTypes()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(SimpleTestGroupMap).Assembly;

        // Act
        services.RegisterMinimalEndpoints(assembly);

        // Assert
        var descriptors = services.Where(sd => sd.ServiceType == typeof(IGroupMap)).ToList();

        foreach (var descriptor in descriptors)
            // Should not register abstract classes or interfaces
            if (descriptor.ImplementationType != null)
            {
                Assert.False(descriptor.ImplementationType.IsAbstract);
                Assert.False(descriptor.ImplementationType.IsInterface);
            }

    }

    [Fact]
    public void RegisterMinimalEndpoints_UsesTryAddEnumerable_ToAvoidDuplicates()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(SimpleTestGroupMap).Assembly;

        // Act
        services.RegisterMinimalEndpoints(assembly);
        services.RegisterMinimalEndpoints(assembly); // Call again

        // Assert
        var descriptors = services.Where(sd => sd.ServiceType == typeof(IGroupMap) &&
                                               sd.ImplementationType == typeof(SimpleTestGroupMap)).ToList();
        Assert.Single(descriptors);
    }

    [Fact]
    public void RegisterMinimalEndpoints_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(SimpleTestGroupMap).Assembly;

        // Act
        var result = services.RegisterMinimalEndpoints(assembly);

        // Assert
        Assert.Same(services, result);
    }
}

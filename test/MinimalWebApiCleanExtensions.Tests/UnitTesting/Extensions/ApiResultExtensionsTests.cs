using MinimalWebApiCleanExtensions.Extensions;
using MinimalWebApiCleanExtensions.Interfaces;
using MinimalWebApiCleanExtensions.ResultPattern;

namespace MinimalWebApiCleanExtensions.Tests.UnitTesting.Extensions;

public class ApiResultExtensionsTests
{
    [Fact]
    public void ToResponse_WithSuccessResult_ReturnsOkResult()
    {
        // Arrange
        var value = "test";
        var result = Result<string>.Success(value);

        // Act
        var response = result.ToResponse();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponse_WithCreatedResult_ReturnsCreatedResult()
    {
        // Arrange
        var value = "test";
        var result = Result<string>.Created(value);

        // Act
        var response = result.ToResponse();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponse_WithFailureBadRequestError_ReturnsProblemResult()
    {
        // Arrange
        var error = new BadRequestError("Bad Request", "Invalid input");
        var result = Result<string>.Failure(error);

        // Act
        var response = result.ToResponse();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponse_WithFailureDomainError_ReturnsProblemResult()
    {
        // Arrange
        var error = new DomainError("Domain Error", "Business rule violation");
        var result = Result<string>.Failure(error);

        // Act
        var response = result.ToResponse();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponse_WithFailureNotFoundError_ReturnsProblemResult()
    {
        // Arrange
        var error = new NotFoundError("Not Found", "Resource not found");
        var result = Result<string>.Failure(error);

        // Act
        var response = result.ToResponse();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponse_WithFailureConflictError_ReturnsProblemResult()
    {
        // Arrange
        var error = new ConflictError("Conflict", "Resource conflict");
        var result = Result<string>.Failure(error);

        // Act
        var response = result.ToResponse();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponse_WithFailureUnauthorizedError_ReturnsUnauthorizedResult()
    {
        // Arrange
        var error = new UnauthorizedError();
        var result = Result<string>.Failure(error);

        // Act
        var response = result.ToResponse();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponse_WithFailureForbiddenError_ReturnsForbiddenResult()
    {
        // Arrange
        var error = new ForbiddenError();
        var result = Result<string>.Failure(error);

        // Act
        var response = result.ToResponse();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponse_WithUnhandledError_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var error = new TestError();
        var result = Result<string>.Failure(error);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => result.ToResponse());
    }

    [Fact]
    public void ToResponse_WithNoContentResult_ReturnsNoContentResult()
    {
        // Arrange
        var result = Result<string>.Success(null);

        // Act
        var response = result.ToResponse();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponseGeneric_WithSuccessResult_ReturnsOkResult()
    {
        // Arrange
        var dto = new TestDto { Id = 1, Name = "Test" };
        var result = Result<TestDto>.Success(dto);

        // Act
        var response = result.ToResponse<TestDto, TestResponse>();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponseGeneric_WithCreatedResult_ReturnsCreatedResult()
    {
        // Arrange
        var dto = new TestDto { Id = 1, Name = "Test" };
        var result = Result<TestDto>.Created(dto);

        // Act
        var response = result.ToResponse<TestDto, TestResponse>();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponseGeneric_WithFailureError_ReturnsProblemResult()
    {
        // Arrange
        var error = new BadRequestError("Bad Request", "Invalid input");
        var result = Result<TestDto>.Failure(error);

        // Act
        var response = result.ToResponse<TestDto, TestResponse>();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponseGenericEnumerable_WithSuccessResult_ReturnsOkResult()
    {
        // Arrange
        var dtos = new List<TestDto>
        {
            new() { Id = 1, Name = "Test1" },
            new() { Id = 2, Name = "Test2" }
        };
        var result = Result<IEnumerable<TestDto>>.Success(dtos);

        // Act
        var response = result.ToResponse<TestDto, TestResponse>();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponseGenericEnumerable_WithCreatedResult_ReturnsCreatedResult()
    {
        // Arrange
        var dtos = new List<TestDto>
        {
            new() { Id = 1, Name = "Test1" },
            new() { Id = 2, Name = "Test2" }
        };
        var result = Result<IEnumerable<TestDto>>.Created(dtos);

        // Act
        var response = result.ToResponse<TestDto, TestResponse>();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponseGenericEnumerable_WithFailureError_ReturnsProblemResult()
    {
        // Arrange
        var error = new BadRequestError("Bad Request", "Invalid input");
        var result = Result<IEnumerable<TestDto>>.Failure(error);

        // Act
        var response = result.ToResponse<TestDto, TestResponse>();

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public void ToResponseGenericEnumerable_WithNullValue_ReturnsEmptyCollection()
    {
        // Arrange
        var result = Result<IEnumerable<TestDto>>.Success(null);

        // Act
        var response = result.ToResponse<TestDto, TestResponse>();

        // Assert
        Assert.NotNull(response);
    }
}

public class TestDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class TestResponse : IBaseResponse<TestDto, TestResponse>
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public static TestResponse? ToResponseModel(TestDto? dto)
    {
        if (dto == null) return null;
        return new TestResponse { Id = dto.Id, Name = dto.Name };
    }
}

public record TestError : Error
{
    public TestError() : base("Test Error", "This is a test error") { }
}
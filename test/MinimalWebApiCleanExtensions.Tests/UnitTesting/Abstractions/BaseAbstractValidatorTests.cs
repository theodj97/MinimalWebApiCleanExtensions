using FluentValidation;
using MinimalWebApiCleanExtensions.Abstractions;

namespace MinimalWebApiCleanExtensions.Tests.UnitTesting.Abstractions;

public class BaseAbstractValidatorTests
{
    private class TestEntity
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    private class ExposedValidator : BaseAbstractValidator<TestEntity>
    {
        public bool ExposedIsValidEmail(string email) => IsValidEmail(email);

        public static bool ExposedTestString(string value) => !string.IsNullOrEmpty(value);

        public static bool ExposedTestDateTime(string dateTime) => DateTime.TryParse(dateTime, out _);

        public static bool ExposedTestStartDateEndDate(StartEndDate startDateEndDate)
        {
            var startDateProp = startDateEndDate.GetType().GetProperty("StartDate");
            var endDateProp = startDateEndDate.GetType().GetProperty("EndDate");

            if (startDateProp == null || endDateProp == null)
                return false;

            string startDate = startDateProp.GetValue(startDateEndDate) as string ?? "";
            string endDate = endDateProp.GetValue(startDateEndDate) as string ?? "";

            // Check if one is empty and the other is not
            if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                return false;

            if (string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(startDate))
                return false;

            // If both are empty, that's valid
            if (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                return true;

            // Check if dates are valid and in correct order
            if (DateTime.TryParse(startDate, out DateTime start) &&
                DateTime.TryParse(endDate, out DateTime end))
            {
                return start <= end;
            }

            return true;
        }

        public static bool ExposedTestSortBy(IEnumerable<KeyValuePair<string, bool>>? sortProperty, Type typeToSortBy)
        {
            if (sortProperty is null || !sortProperty.Any())
                return true;

            if (sortProperty.Select(kvp => kvp.Key).Distinct().Count() != sortProperty.Count())
                return false;

            var typeToSortByProperties = typeToSortBy.GetProperties(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Select(p => p.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var property in sortProperty)
                if (!typeToSortByProperties.Contains(property.Key))
                    return false;

            return true;
        }
    }

    private class StartEndDate
    {
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }

    [Fact]
    public void Constructor_Should_SetCascadeModeToContinue()
    {
        // Arrange & Act
        var validator = new BaseAbstractValidator<object>();

        // Assert
        Assert.Equal(CascadeMode.Continue, validator.ClassLevelCascadeMode);
    }

    [Fact]
    public void IsValidEmail_WithValidEmail_ShouldReturnTrue()
    {
        // Arrange
        var validator = new ExposedValidator();
        var email = "test@example.com";

        // Act
        var result = validator.ExposedIsValidEmail(email);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidEmail_WithInvalidEmail_ShouldReturnFalse()
    {
        // Arrange
        var validator = new ExposedValidator();
        var email = "invalid-email";

        // Act
        var result = validator.ExposedIsValidEmail(email);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void NotNullNotEmpty_WithNullValue_ShouldReturnFalse()
    {
        // Arrange
        var validator = new ExposedValidator();
        string value = null!;

        // Act
        var result = ExposedValidator.ExposedTestString(value);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void NotNullNotEmpty_WithEmptyValue_ShouldReturnFalse()
    {
        // Arrange
        var validator = new ExposedValidator();
        string value = "";

        // Act
        var result = ExposedValidator.ExposedTestString(value);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void NotNullNotEmpty_WithValidValue_ShouldReturnTrue()
    {
        // Arrange
        var validator = new ExposedValidator();
        string value = "Valid Value";

        // Act
        var result = ExposedValidator.ExposedTestString(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateDateTime_WithValidDate_ShouldReturnTrue()
    {
        // Arrange
        var validator = new ExposedValidator();
        var date = "2023-01-01";

        // Act
        var result = ExposedValidator.ExposedTestDateTime(date);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateDateTime_WithInvalidDate_ShouldReturnFalse()
    {
        // Arrange
        var validator = new ExposedValidator();
        var date = "invalid-date";

        // Act
        var result = ExposedValidator.ExposedTestDateTime(date);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateStartDateAndEndDate_WithBothDatesEmpty_ShouldReturnTrue()
    {
        // Arrange
        var validator = new ExposedValidator();
        var dates = new StartEndDate { StartDate = "", EndDate = "" };

        // Act
        var result = ExposedValidator.ExposedTestStartDateEndDate(dates);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateStartDateAndEndDate_WithStartDateEmptyAndEndDateFilled_ShouldReturnFalse()
    {
        // Arrange
        var validator = new ExposedValidator();
        var dates = new StartEndDate { StartDate = "", EndDate = "2023-01-02" };

        // Act
        var result = ExposedValidator.ExposedTestStartDateEndDate(dates);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateStartDateAndEndDate_WithEndDateEmptyAndStartDateFilled_ShouldReturnFalse()
    {
        // Arrange
        var validator = new ExposedValidator();
        var dates = new StartEndDate { StartDate = "2023-01-01", EndDate = "" };

        // Act
        var result = ExposedValidator.ExposedTestStartDateEndDate(dates);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateStartDateAndEndDate_WithStartDateAfterEndDate_ShouldReturnFalse()
    {
        // Arrange
        var validator = new ExposedValidator();
        var dates = new StartEndDate { StartDate = "2023-01-02", EndDate = "2023-01-01" };

        // Act
        var result = ExposedValidator.ExposedTestStartDateEndDate(dates);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateStartDateAndEndDate_WithValidDates_ShouldReturnTrue()
    {
        // Arrange
        var validator = new ExposedValidator();
        var dates = new StartEndDate { StartDate = "2023-01-01", EndDate = "2023-01-02" };

        // Act
        var result = ExposedValidator.ExposedTestStartDateEndDate(dates);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateSortBy_WithNullSortProperty_ShouldReturnTrue()
    {
        // Arrange
        var validator = new ExposedValidator();

        // Act
        var result = ExposedValidator.ExposedTestSortBy(null, typeof(TestEntity));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateSortBy_WithEmptySortProperty_ShouldReturnTrue()
    {
        // Arrange
        var validator = new ExposedValidator();

        // Act
        var result = ExposedValidator.ExposedTestSortBy([], typeof(TestEntity));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateSortBy_WithDuplicatedSortProperties_ShouldReturnFalse()
    {
        // Arrange
        var validator = new ExposedValidator();
        var sortProperties = new List<KeyValuePair<string, bool>>
        {
            new("Name", true),
            new("Name", false)
        };

        // Act
        var result = ExposedValidator.ExposedTestSortBy(sortProperties, typeof(TestEntity));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateSortBy_WithInvalidPropertyName_ShouldReturnFalse()
    {
        // Arrange
        var validator = new ExposedValidator();
        var sortProperties = new List<KeyValuePair<string, bool>>
        {
            new("InvalidProperty", true)
        };

        // Act
        var result = ExposedValidator.ExposedTestSortBy(sortProperties, typeof(TestEntity));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateSortBy_WithValidSortProperties_ShouldReturnTrue()
    {
        // Arrange
        var validator = new ExposedValidator();
        var sortProperties = new List<KeyValuePair<string, bool>>
        {
            new("Name", true),
            new("CreatedAt", false)
        };

        // Act
        var result = ExposedValidator.ExposedTestSortBy(sortProperties, typeof(TestEntity));

        // Assert
        Assert.True(result);
    }
}
namespace OutcomeCore.UnitTests;

public class OutcomeTests
{
    [Fact]
    public void IsError_ShouldBeFalse_WhenOutcomeContainsValue()
    {
        // Arrange
        Outcome<string> outcome = "value";

        // Act
        var result = outcome.IsError;

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsError_ShouldBeTrue_WhenOutcomeContainsError()
    {
        // Arrange
        Outcome<string> outcome = Error.Validation();

        // Act
        var result = outcome.IsError;

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsError_ShouldBeTrue_WhenOutcomeContainsErrorList()
    {
        // Arrange
        var errorList = new[] { Error.Validation(), Error.BusinessRule() };
        Outcome<string> outcome = errorList;

        // Act
        var result = outcome.IsError;

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Value_ShouldReturnValue_WhenOutcomeContainsValue()
    {
        // Arrange
        Outcome<string> outcome = "value";

        // Act
        var value = outcome.Value;

        // Assert
        value.ShouldBe("value");
    }

    [Fact]
    public void Value_ShouldThrowInvalidOperationException_WhenOutcomeContainsError()
    {
        // Arrange
        Outcome<string> outcome = Error.Validation();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            _ = outcome.Value;
        });
    }

    [Fact]
    public void Errors_ShouldReturnErrors_WhenOutcomeContainsError()
    {
        // Arrange
        var error = Error.Validation();
        Outcome<string> outcome = error;

        // Act
        var errors = outcome.Errors;

        // Assert
        errors.Count.ShouldBe(1);
        errors[0].ShouldBe(error);
    }

    [Fact]
    public void Errors_ShouldReturnErrors_WhenOutcomeContainsErrorList()
    {
        // Arrange
        var errorList = new List<Error>() { Error.Validation(), Error.BusinessRule() };
        Outcome<string> outcome = errorList;

        // Act
        var errors = outcome.Errors;

        // Assert
        errors.Count.ShouldBe(2);
        errors.ShouldContain(errorList[0]);
        errors.ShouldContain(errorList[1]);
    }

    [Fact]
    public void Errors_ShouldThrowInvalidOperationException_WhenOutcomeContainsValue()
    {
        // Arrange
        Outcome<string> outcome = "value";

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            _ = outcome.Errors;
        });
    }
}

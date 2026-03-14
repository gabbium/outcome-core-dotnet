namespace OutcomeCore.UnitTests;

public class OutcomeVoidTests
{
    [Fact]
    public void IsError_ShouldBeFalse_WhenOutcomeIsSuccess()
    {
        // Arrange
        var outcome = Outcome.Success();

        // Act
        var result = outcome.IsError;

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void IsError_ShouldBeTrue_WhenOutcomeCreatedWithFailure()
    {
        // Arrange
        var error = Error.Validation();
        var outcome = Outcome.Failure(error);

        // Act
        var result = outcome.IsError;

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsError_ShouldBeTrue_WhenOutcomeCreatedWithFailureParams()
    {
        // Arrange
        var outcome = Outcome.Failure(
            Error.Validation(),
            Error.BusinessRule());

        // Act
        var result = outcome.IsError;

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsError_ShouldBeTrue_WhenOutcomeContainsErrorImplicit()
    {
        // Arrange
        Outcome outcome = Error.Validation();

        // Act
        var result = outcome.IsError;

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void IsError_ShouldBeTrue_WhenOutcomeContainsErrorListImplicit()
    {
        // Arrange
        var errorList = new[] { Error.Validation(), Error.BusinessRule() };
        Outcome outcome = errorList;

        // Act
        var result = outcome.IsError;

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Errors_ShouldReturnErrors_WhenOutcomeContainsError()
    {
        // Arrange
        var error = Error.Validation();
        Outcome outcome = error;

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
        var errorList = new List<Error> { Error.Validation(), Error.BusinessRule() };
        Outcome outcome = errorList;

        // Act
        var errors = outcome.Errors;

        // Assert
        errors.Count.ShouldBe(2);
        errors.ShouldContain(errorList[0]);
        errors.ShouldContain(errorList[1]);
    }

    [Fact]
    public void Errors_ShouldThrowInvalidOperationException_WhenOutcomeIsSuccess()
    {
        // Arrange
        var outcome = Outcome.Success();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            _ = outcome.Errors;
        });
    }
}

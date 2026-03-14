namespace OutcomeCore.UnitTests;

public class ErrorTests
{
    [Fact]
    public void Validation_ShouldCreateValidationError_WithDefaultValues()
    {
        // Act
        var error = Error.Validation();

        // Assert
        error.Code.ShouldBe("validation");
        error.Description.ShouldBe("The request is invalid.");
        error.Type.ShouldBe(ErrorType.Validation);
        error.Metadata.ShouldBeNull();
    }

    [Fact]
    public void BusinessRule_ShouldCreateBusinessRuleError_WithDefaultValues()
    {
        // Act
        var error = Error.BusinessRule();

        // Assert
        error.Code.ShouldBe("business-rule");
        error.Description.ShouldBe("A business rule was violated.");
        error.Type.ShouldBe(ErrorType.BusinessRule);
        error.Metadata.ShouldBeNull();
    }

    [Fact]
    public void NotFound_ShouldCreateNotFoundError_WithDefaultValues()
    {
        // Act
        var error = Error.NotFound();

        // Assert
        error.Code.ShouldBe("not-found");
        error.Description.ShouldBe("The requested resource was not found.");
        error.Type.ShouldBe(ErrorType.NotFound);
        error.Metadata.ShouldBeNull();
    }

    [Fact]
    public void Conflict_ShouldCreateConflictError_WithDefaultValues()
    {
        // Act
        var error = Error.Conflict();

        // Assert
        error.Code.ShouldBe("conflict");
        error.Description.ShouldBe("A conflict occurred with the current state of the resource.");
        error.Type.ShouldBe(ErrorType.Conflict);
        error.Metadata.ShouldBeNull();
    }
}

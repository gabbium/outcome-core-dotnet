namespace OutcomeCore;

public record Error
{
    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }
    public Dictionary<string, object>? Metadata { get; }

    private Error(string code, string description, ErrorType type, Dictionary<string, object>? metadata)
    {
        Code = code;
        Description = description;
        Type = type;
        Metadata = metadata;
    }

    public static Error Validation(
        string code = "validation",
        string description = "The request is invalid.",
        Dictionary<string, object>? metadata = null)
        => new(code, description, ErrorType.Validation, metadata);

    public static Error BusinessRule(
        string code = "business-rule",
        string description = "A business rule was violated.",
        Dictionary<string, object>? metadata = null)
        => new(code, description, ErrorType.BusinessRule, metadata);

    public static Error NotFound(
        string code = "not-found",
        string description = "The requested resource was not found.",
        Dictionary<string, object>? metadata = null)
        => new(code, description, ErrorType.NotFound, metadata);

    public static Error Conflict(
        string code = "conflict",
        string description = "A conflict occurred with the current state of the resource.",
        Dictionary<string, object>? metadata = null)
        => new(code, description, ErrorType.Conflict, metadata);

}

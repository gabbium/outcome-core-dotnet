namespace OutcomeCore;

public class Outcome : IOutcome
{
    private readonly IReadOnlyList<Error>? _errors = null;

    public bool IsError => _errors is not null;

    public IReadOnlyList<Error> Errors => IsError
        ? _errors!
        : throw new InvalidOperationException("Outcome does not contain errors.");

    private Outcome()
    {
    }

    private Outcome(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        _errors = [error];
    }

    private Outcome(List<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
        {
            throw new ArgumentException("Errors collection cannot be empty.", nameof(errors));
        }

        _errors = errors;
    }

    public static Outcome Success() => new();

    public static Outcome Failure(Error error) => new(error);

    public static Outcome Failure(List<Error> errors) => new(errors);

    public static Outcome Failure(params Error[] errors) => new([.. errors]);

    public static implicit operator Outcome(Error error)
    {
        return new Outcome(error);
    }

    public static implicit operator Outcome(List<Error> errors)
    {
        return new Outcome(errors);
    }

    public static implicit operator Outcome(Error[] errors)
    {
        return new Outcome([.. errors]);
    }
}


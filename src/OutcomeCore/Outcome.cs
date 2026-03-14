namespace OutcomeCore;

public class Outcome<TValue> : IOutcome<TValue>
{
    private readonly TValue? _value = default;
    private readonly IReadOnlyList<Error>? _errors = null;

    public bool IsError => _errors is not null;

    public IReadOnlyList<Error> Errors => IsError
        ? _errors!
        : throw new InvalidOperationException("Outcome does not contain errors.");

    public TValue Value => IsError
        ? throw new InvalidOperationException("Outcome does not contain a value.")
        : _value!;

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

    private Outcome(TValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _value = value;
    }

    public static implicit operator Outcome<TValue>(TValue value)
    {
        return new Outcome<TValue>(value);
    }

    public static implicit operator Outcome<TValue>(Error error)
    {
        return new Outcome<TValue>(error);
    }

    public static implicit operator Outcome<TValue>(List<Error> errors)
    {
        return new Outcome<TValue>(errors);
    }

    public static implicit operator Outcome<TValue>(Error[] errors)
    {
        return new Outcome<TValue>([.. errors]);
    }
}

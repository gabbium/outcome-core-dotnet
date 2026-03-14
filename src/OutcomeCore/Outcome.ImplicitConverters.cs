namespace OutcomeCore;

public partial class Outcome<TValue> : IOutcome<TValue>
{
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

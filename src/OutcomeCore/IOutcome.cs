namespace OutcomeCore;

public interface IOutcome
{
    IReadOnlyList<Error> Errors { get; }
    bool IsError { get; }
}

public interface IOutcome<out TValue> : IOutcome
{
    TValue Value { get; }
}

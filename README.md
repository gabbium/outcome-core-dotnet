# OutcomeCore

![NuGet Version](https://img.shields.io/nuget/v/OutcomeCore)
![NuGet Downloads](https://img.shields.io/nuget/dt/OutcomeCore)
![Sonar Quality Gate](https://img.shields.io/sonar/quality_gate/gabbium_outcome-core-dotnet?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Coverage](https://img.shields.io/sonar/coverage/gabbium_outcome-core-dotnet?server=https%3A%2F%2Fsonarcloud.io)

A lightweight **result pattern library for .NET** that represents operation outcomes without relying on exceptions for control flow.

`OutcomeCore` provides a simple and explicit way to return either:

- a **value**
- or **one or more errors**

This approach improves readability, encourages explicit error handling, and integrates naturally with modern .NET architectures such as **CQRS, handlers, and Minimal APIs**.

Inspired by patterns used in libraries such as ErrorOr and Ardalis.Result.

---

# Installation

```bash
dotnet add package OutcomeCore
```

---

# Core Concepts

An operation returns either:

```
Value
or
Errors
```

Never both.

```csharp
Outcome<T>
```

If `IsError` is `true`, the operation failed.

If `IsError` is `false`, the operation succeeded and `Value` is available.

---

# Error Model

Errors are strongly typed and categorized.

```csharp
public record Error
{
    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }
    public Dictionary<string, object>? Metadata { get; }
}
```

Supported error types:

```
Validation
BusinessRule
NotFound
Conflict
```

Example:

```csharp
return Error.Validation(
    code: "email.invalid",
    description: "Email format is invalid"
);
```

---

# Handler Example (Recommended Usage)

`OutcomeCore` works naturally with **CQRS handlers**.

### Query Handler

```csharp
public sealed class GetBookHandler
{
    private readonly IBookRepository _repository;

    public GetBookHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public Outcome<Book> Handle(GetBookQuery query)
    {
        var book = _repository.GetById(query.Id);

        if (book is null)
        {
            return Error.NotFound(
                code: "book.not_found",
                description: $"Book {query.Id} was not found"
            );
        }

        return book;
    }
}
```

---

### Command Handler

Commands often return `Unit` when no value is needed.

```csharp
public sealed class DeleteBookHandler
{
    private readonly IBookRepository _repository;

    public DeleteBookHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public Outcome<Unit> Handle(DeleteBookCommand command)
    {
        var exists = _repository.Exists(command.Id);

        if (!exists)
        {
            return Error.NotFound(
                code: "book.not_found",
                description: $"Book {command.Id} does not exist"
            );
        }

        _repository.Delete(command.Id);

        return Unit.Value;
    }
}
```

---

# Returning Multiple Errors

```csharp
return new[]
{
    Error.Validation("title.required", "Title is required"),
    Error.Validation("author.required", "Author is required")
};
```

---

# Validation Pipeline Integration

`OutcomeCore` integrates naturally with validation pipelines commonly used in **CQRS architectures**.

When combined with validation libraries such as **FluentValidation**, validation errors can be converted into `Outcome` errors before the handler executes.

Example validation behavior:

```csharp
public class ValidationBehavior<TMessage, TResponse>(
    IEnumerable<IValidator<TMessage>> validators)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : IOutcome
{
    private readonly IValidator<TMessage>[] _validators = [.. validators];

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Length != 0)
        {
            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(new ValidationContext<TMessage>(message), cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Count != 0)
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Count != 0)
            {
                var errors = failures
                    .ConvertAll(error => Error.Validation(
                        code: error.PropertyName,
                        description: error.ErrorMessage));

                return (dynamic)errors;
            }
        }

        return await next(message, cancellationToken);
    }
}
```

Validation errors will automatically propagate as `Outcome` failures.

---

# Mapping Outcome to HTTP Responses

When using **ASP.NET Minimal APIs**, `Outcome` can be mapped to HTTP responses using a helper.

Example mapper:

```csharp
public static class CustomResults
{
    public static IResult Problem(IOutcome outcome)
    {
        if (!outcome.IsError)
        {
            throw new InvalidOperationException("Cannot return Problem for a successful outcome.");
        }

        var errors = outcome.Errors;

        if (errors.Count > 1)
        {
            return Results.ValidationProblem(
                errors: ToValidationDictionary(errors),
                title: "One or more validation errors occurred",
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        var firstError = errors[0];

        return Results.Problem(
            title: GetTitle(firstError.Type),
            detail: firstError.Description,
            type: GetType(firstError.Type),
            statusCode: GetStatusCode(firstError.Type)
        );
    }
}
```

Example endpoint:

```csharp
private static async Task<IResult> HandleAsync(
    CreateBookRequest request,
    IMediator mediator,
    CancellationToken cancellationToken)
{
    var command = new CreateBookCommand(
        request.Title,
        request.Author,
        request.Description,
        request.PublishedYear);

    var result = await mediator.Send(command, cancellationToken);

    if (result.IsError)
    {
        return CustomResults.Problem(result);
    }

    return Results.Created($"/books/{result.Value.Id}", result.Value);
}
```

This keeps endpoints simple and delegates HTTP mapping to a dedicated component.

---

# Handling Results

```csharp
var result = handler.Handle(command);

if (result.IsError)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.Code}: {error.Description}");
    }

    return;
}

var value = result.Value;
```

---

# Implicit Conversions

`OutcomeCore` provides implicit conversions to reduce boilerplate.

You can return values or errors directly:

```csharp
return book;
```

```csharp
return Error.NotFound();
```

```csharp
return new[]
{
    Error.Validation(),
    Error.BusinessRule()
};
```

These automatically convert to `Outcome<T>`.

---

# Unit Type

When an operation succeeds but does not return a value, use `Unit`.

```csharp
public Outcome<Unit> Handle(CreateUserCommand command)
{
    repository.Add(new User(command.Name));

    return Unit.Value;
}
```

---

# Example Flow

```
Endpoint
   ↓
Mediator
   ↓
Handler
   ↓
Outcome<T>
   ↓
HTTP mapping
   ↓
HTTP Response
```

---

# License

This project is licensed under the MIT License. See the
[LICENSE](LICENSE) file for details.

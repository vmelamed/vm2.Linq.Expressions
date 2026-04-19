namespace vm2.Linq.Expressions.Serialization.Exceptions;

/// <summary>
/// Thrown when validation of an expression document against its schema fails.
/// </summary>
/// <param name="message">The exception message.</param>
/// <param name="inner">The inner exception.</param>
[ExcludeFromCodeCoverage]
public class SchemaValidationErrorsException(string? message = null, Exception? inner = null)
    : Exception(message ?? "Validation against the schema failed.", inner);

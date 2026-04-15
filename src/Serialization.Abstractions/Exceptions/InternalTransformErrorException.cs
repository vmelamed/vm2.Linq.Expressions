namespace vm2.Linq.Expressions.Serialization.Exceptions;

/// <summary>
/// Indicates an unexpected internal error during a transform operation.
/// </summary>
/// <param name="message">The exception message.</param>
/// <param name="inner">The inner exception.</param>
public class InternalTransformErrorException(string? message = null, Exception? inner = null)
    : InvalidOperationException(message ?? "Unexpected transform error.", inner);

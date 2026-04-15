namespace vm2.Linq.Expressions.Serialization.Json;

/// <summary>
/// Represents an exception that is thrown when an error occurs during the JSON serialization or deserialization of an
/// expression, usually when retrieving <see cref="JsonObject"/>, <see cref="JsonArray"/>, or <see cref="JsonValue"/>.
/// This is very unlikely to be thrown during normal operation.
/// </summary>
public class ExpressionJsonSerializationException(string? message = null, Exception? innerException = null)
                : InvalidOperationException(message ?? defaultMessage, innerException)
{
    const string defaultMessage = "An error occurred during expression JSON serialization or deserialization.";
}

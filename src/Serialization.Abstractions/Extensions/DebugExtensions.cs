namespace vm2.Linq.Expressions.Serialization.Extensions;

/// <summary>
/// Debug output helper extensions.
/// </summary>
[ExcludeFromCodeCoverage]
public static class DebugExtensions
{
    /// <summary>
    /// Provides a scoped debug context that automatically adjusts indentation in debug output.
    /// </summary>
    public class DebugScope : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugScope"/> class.
        /// </summary>
        public DebugScope(string scopeName)
        {
            Debug.WriteLine($"==== {scopeName}");
            Debug.Indent();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Debug.Unindent();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Returns a <see cref="DebugScope"/> that indents debug output for the lifetime of the scope.
    /// </summary>
    /// <param name="scopeName">Name of the scope.</param>
    public static DebugScope OutputDebugScope(string scopeName) => new(scopeName);
}

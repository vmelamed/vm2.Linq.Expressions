namespace vm2.Linq.Expressions.Serialization;

/// <summary>
/// Options for serializing/deserializing documents.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class DocumentOptions
{
    /// <summary>
    /// Updates the specified field to a new value and indicates whether the document options has changed.
    /// </summary>
    /// <typeparam name="T">The type of the field, which must be not null.</typeparam>
    /// <param name="field">The current value of the field.</param>
    /// <param name="value">The new value to assign to the field.</param>
    /// <returns>The new value of the field.</returns>
    protected T Change<T>(T @field, T @value) where T : notnull
    {
        Changed |= !@field.Equals(@value);
        return @value;
    }

    /// <summary>
    /// Determines whether this instance has changed since the last read of <see cref="Changed"/>.
    /// Reading this property resets it to <see langword="false"/>.
    /// </summary>
    public bool Changed
    {
        get
        {
            var changed = field;
            field = false;
            return changed;
        }
        protected set;
    }

    /// <summary>
    /// Gets the identifiers transformation convention.
    /// </summary>
    public IdentifierConventions Identifiers { get; set => field = Change(field, value); } = IdentifierConventions.Preserve;

    /// <summary>
    /// Gets the type transform convention.
    /// </summary>
    public TypeNameConventions TypeNames { get; set => field = Change(field, value); } = TypeNameConventions.FullName;

    /// <summary>
    /// Gets or sets a value indicating whether to indent the output document.
    /// </summary>
    public bool Indent { get; set => field = Change(field, value); } = true;

    /// <summary>
    /// Gets or sets the size of the document's tab indention.
    /// </summary>
    public int IndentSize { get; set => field = Change(field, value); } = 2;

    /// <summary>
    /// Gets or sets a value indicating whether to add comments to the resultant node.
    /// </summary>
    public bool AddComments { get; set => field = Change(field, value); }

    /// <summary>
    /// Gets or sets a value indicating whether the lambda types should be serialized to the output document.
    /// </summary>
    public bool AddLambdaTypes { get; set => field = Change(field, value); }

    /// <summary>
    /// Gets or sets a value indicating whether to validate input documents that are to be transformed to <see cref="Expression"/>-s.
    /// </summary>
    public ValidateExpressionDocuments ValidateInputDocuments { get; set => field = Change(field, value); } = ValidateExpressionDocuments.IfSchemaPresent;

    /// <summary>
    /// Determines whether the expression schema was added.
    /// </summary>
    public abstract bool HasExpressionSchema { get; }

    /// <summary>
    /// Transforms the <paramref name="type"/> to a readable string according to the <see cref="TypeNames"/> convention.
    /// </summary>
    /// <param name="type">The type to be transformed to a readable string.</param>
    /// <returns>The human readable transformation of the parameter <paramref name="type"/>.</returns>
    public string TransformTypeName(Type type)
        => Transform.TypeName(type, TypeNames);

    /// <summary>
    /// Transforms the <paramref name="identifier"/> according to the <see cref="Identifiers"/> conventions.
    /// </summary>
    /// <param name="identifier">The identifier to be transformed.</param>
    /// <returns>The transformed <paramref name="identifier"/>.</returns>
    public string TransformIdentifier(string identifier)
        => Transform.Identifier(identifier, Identifiers);

    /// <summary>
    /// Determines whether to validate input documents against the expressions schema.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The expressions schema was not added — use the appropriate SetSchemaLocation method.
    /// </exception>
    public bool MustValidate
        => ValidateInputDocuments == ValidateExpressionDocuments.Always
                ? HasExpressionSchema ? true : throw new InvalidOperationException("The expressions schema was not added — use the appropriate SetSchemaLocation or LoadSchema method.")
                : ValidateInputDocuments == ValidateExpressionDocuments.IfSchemaPresent && HasExpressionSchema;
}

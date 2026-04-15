namespace vm2.Linq.Expressions.Serialization.Conventions;

/// <summary>
/// Provides identifier and type-name transformations according to configurable conventions.
/// </summary>
public static partial class Transform
{
    /*
    static readonly UnicodeCategory[] _beginIdentifierArr_ =
    [
        UnicodeCategory.UppercaseLetter,
        UnicodeCategory.LowercaseLetter,
        UnicodeCategory.ModifierLetter,
        UnicodeCategory.TitlecaseLetter,
        UnicodeCategory.OtherLetter,
        UnicodeCategory.LetterNumber,
        UnicodeCategory.ConnectorPunctuation,
        UnicodeCategory.SpacingCombiningMark,
        UnicodeCategory.Format
    ];
    static readonly FrozenSet<UnicodeCategory> _beginIdentifier = _beginIdentifierArr_.ToFrozenSet();

    static readonly UnicodeCategory[] _beginWordIdentifierArr_ =
    [
        UnicodeCategory.UppercaseLetter,
        UnicodeCategory.LowercaseLetter,
        UnicodeCategory.ModifierLetter,
        UnicodeCategory.TitlecaseLetter,
        UnicodeCategory.OtherLetter,
        UnicodeCategory.LetterNumber,
        UnicodeCategory.ConnectorPunctuation,
        UnicodeCategory.SpacingCombiningMark,
        UnicodeCategory.Format,
        UnicodeCategory.DecimalDigitNumber,
    ];
    static readonly FrozenSet<UnicodeCategory> _beginWordIdentifier = _beginWordIdentifierArr_.ToFrozenSet();
    */

    static readonly UnicodeCategory[] _restWordIdentifierArr_ =
    [
        UnicodeCategory.LowercaseLetter,
        UnicodeCategory.ModifierLetter,
        UnicodeCategory.TitlecaseLetter,
        UnicodeCategory.OtherLetter,
        UnicodeCategory.LetterNumber,
        UnicodeCategory.ConnectorPunctuation,
        UnicodeCategory.SpacingCombiningMark,
        UnicodeCategory.Format,
        UnicodeCategory.DecimalDigitNumber,
    ];
    static readonly FrozenSet<UnicodeCategory> _restWordIdentifier = _restWordIdentifierArr_.ToFrozenSet();

    [GeneratedRegex(@"^@?([\p{L}_])([\p{Ll}\p{Nd}\p{Pc}]*)(([\p{Lu}_])([\p{Ll}\p{Nd}\p{Pc}]*))*$")]
    private static partial Regex CSharpIdentifier();

    /// <summary>
    /// Transforms the identifier according to the specified <see cref="IdentifierConventions"/>.
    /// </summary>
    /// <param name="identifier">The identifier to transform.</param>
    /// <param name="convention">The naming convention.</param>
    /// <returns>The transformed identifier.</returns>
    /// <exception cref="InternalTransformErrorException">Invalid identifier.</exception>
    public static string Identifier(string identifier, IdentifierConventions convention)
    {
        if (!CSharpIdentifier().IsMatch(identifier))
            throw new InternalTransformErrorException("Invalid identifier.");

        if (convention == IdentifierConventions.Preserve)
            return identifier;

        var chars = identifier.AsSpan();
        var c = chars[0] == '@' ? 1 : 0;
        var len = identifier.Length;
        var xChars = len * 2 < 1024 ? stackalloc char[len * 2] : new char[len * 2];
        var to = 0;

        while (c < len)
        {
            while (c < len && chars[c] == '_')
                c++;

            if (c >= len)
                break;

            var wordStart = c++;

            while (c < len && _restWordIdentifier.Contains(char.GetUnicodeCategory(chars[c])) && chars[c] != '_')
                ++c;

            var word = chars[wordStart..c];
            var xWord = xChars[to..];
            var x = 0;

            switch (convention)
            {
                case IdentifierConventions.Camel:
                    word.CopyTo(xWord);
                    xWord[x] = to == 0 ? char.ToLower(xWord[x]) : char.ToUpper(xWord[x]);
                    break;

                case IdentifierConventions.Pascal:
                    word.CopyTo(xWord);
                    xWord[x] = char.ToUpper(xWord[x]);
                    break;

                case IdentifierConventions.SnakeLower:
                    if (to > 0)
                    {
                        to++;
                        xWord[x++] = '_';
                    }
                    for (var w = 0; w < word.Length; w++)
                        xWord[x++] = char.ToLower(word[w]);
                    break;

                case IdentifierConventions.SnakeUpper:
                    if (to > 0)
                    {
                        to++;
                        xWord[x++] = '_';
                    }
                    for (var w = 0; w < word.Length; w++)
                        xWord[x++] = char.ToUpper(word[w]);
                    break;

                case IdentifierConventions.KebabLower:
                    if (to > 0)
                    {
                        to++;
                        xWord[x++] = '-';
                    }
                    for (var w = 0; w < word.Length; w++)
                        xWord[x++] = char.ToLower(word[w]);
                    break;

                case IdentifierConventions.KebabUpper:
                    if (to > 0)
                    {
                        to++;
                        xWord[x++] = '-';
                    }
                    for (var w = 0; w < word.Length; w++)
                        xWord[x++] = char.ToUpper(word[w]);
                    break;
            }
            to += word.Length;
        }

        return xChars[..to].ToString();
    }
}

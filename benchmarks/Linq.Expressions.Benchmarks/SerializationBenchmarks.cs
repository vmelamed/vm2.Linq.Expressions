// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Linq.Expressions.Benchmarks;

#pragma warning disable CA1822 // Mark members as static

#if SHORT_RUN
[ShortRunJob]
#else
[SimpleJob(RuntimeMoniker.HostProcess)]
#endif
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.Default)]
[JsonExporter]
[MarkdownExporter]
public class SerializationBenchmarks
{
    static readonly ParameterExpression _x = Expression.Parameter(typeof(int), "x");
    static readonly ParameterExpression _y = Expression.Parameter(typeof(int), "y");
    static readonly ParameterExpression _s = Expression.Parameter(typeof(string), "s");
    static readonly ParameterExpression _arr = Expression.Parameter(typeof(int[]), "arr");

    static readonly FrozenDictionary<string, Expression> _cases =
        new Dictionary<string, Expression>
        {
            ["Arithmetic"] = (Expression<Func<int, int, int>>)((x, y) => x * y + 2),
            ["Conditional"] = (Expression<Func<int, int>>)(x => x > 0 ? x : -x),
            ["MethodCall"] = (Expression<Func<string, string>>)(s => s.Trim().ToUpperInvariant()),
            ["LinqCall"] = (Expression<Func<int[], int>>)(arr => arr.Where(n => n > 0).Sum()),
            ["Block"] = BuildBlockExpression(),
        }.ToFrozenDictionary(StringComparer.Ordinal);

    static Expression<Func<int, int, int>> BuildBlockExpression()
    {
        var local = Expression.Variable(typeof(int), "tmp");

        var block = Expression.Block(
            new[] { local },
            Expression.Assign(local, Expression.Add(_x, Expression.Constant(1))),
            Expression.Multiply(local, _y));

        return Expression.Lambda<Func<int, int, int>>(block, _x, _y);
    }

    static readonly Lock _schemasLock = new();
    static string? _jsonSchemaPath;

    ExpressionXmlTransform _xmlTransform = null!;
    ExpressionJsonTransform _jsonTransform = null!;

    Expression _expression = null!;
    XDocument _xmlDoc = null!;
    JsonObject _jsonDoc = null!;

    [ParamsSource(nameof(CaseNames))]
    public string CaseName { get; set; } = string.Empty;

    [Params(ValidateExpressionDocuments.Never, ValidateExpressionDocuments.Always)]
    public ValidateExpressionDocuments ValidateInputDocuments { get; set; }

    public static IEnumerable<string> CaseNames => _cases.Keys;

    [GlobalSetup]
    public void Setup()
    {
        _xmlTransform = new ExpressionXmlTransform(new XmlOptions
        {
            Indent = false,
            AddComments = false,
            ValidateInputDocuments = ValidateInputDocuments,
        });

        _jsonTransform = new ExpressionJsonTransform(new JsonOptions(EnsureValidationSchemas())
        {
            Indent = false,
            AddComments = false,
            ValidateInputDocuments = ValidateInputDocuments,
        });

        _expression = _cases[CaseName];

        // Precompute serialized payloads so deserialize benchmarks measure only deserialize cost.
        var xmlSerializer = new ExpressionXmlTransform(new XmlOptions
        {
            Indent = false,
            AddComments = false,
            ValidateInputDocuments = ValidateExpressionDocuments.Never,
        });

        var jsonSerializer = new ExpressionJsonTransform(new JsonOptions
        {
            Indent = false,
            AddComments = false,
            ValidateInputDocuments = ValidateExpressionDocuments.Never,
        });

        _xmlDoc = xmlSerializer.Transform(_expression);
        _jsonDoc = jsonSerializer.Transform(_expression);

        // Guardrail: fail setup if correctness regresses.
        var xmlRoundTrip = _xmlTransform.Transform(_xmlDoc);
        if (!_expression.DeepEquals(xmlRoundTrip))
            throw new InvalidOperationException($"XML round-trip mismatch for case '{CaseName}'.");

        var jsonRoundTrip = _jsonTransform.Transform(_jsonDoc);
        if (!_expression.DeepEquals(jsonRoundTrip))
            throw new InvalidOperationException($"JSON round-trip mismatch for case '{CaseName}'.");
    }

    static string EnsureValidationSchemas()
    {
        if (_jsonSchemaPath is not null)
            return _jsonSchemaPath;

        lock (_schemasLock)
        {
            if (_jsonSchemaPath is not null)
                return _jsonSchemaPath;

            var repoRoot = FindRepoRoot();
            var xmlSchemaPath = Path.Combine(repoRoot, "src", "Serialization.Xml", "Schema");

            XmlOptions.SetSchemasLocations(
                new Dictionary<string, string?>
                {
                    [XmlOptions.Ser] = Path.Combine(xmlSchemaPath, "Microsoft.Serialization.xsd"),
                    [XmlOptions.Dcs] = Path.Combine(xmlSchemaPath, "DataContract.xsd"),
                    [XmlOptions.Exs] = Path.Combine(xmlSchemaPath, "Linq.Expressions.Serialization.xsd"),
                },
                reset: false);

            _jsonSchemaPath = Path.Combine(repoRoot, "src", "Serialization.Json", "Schema", "Linq.Expressions.Serialization.json");
            return _jsonSchemaPath;
        }
    }

    static string FindRepoRoot()
    {
        var dir = AppContext.BaseDirectory;

        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir, "vm2.Linq.Expressions.slnx")))
                return dir;

            dir = Path.GetDirectoryName(dir);
        }

        throw new InvalidOperationException("Could not find the repository root (looked for vm2.Linq.Expressions.slnx).");
    }

    [Benchmark(Description = "XML serialize")]
    public XDocument Xml_Serialize()
    {
        return _xmlTransform.Transform(_expression);
    }

    [Benchmark(Description = "JSON serialize")]
    public JsonObject Json_Serialize()
    {
        return _jsonTransform.Transform(_expression);
    }

    [Benchmark(Description = "XML deserialize")]
    public Expression Xml_Deserialize()
    {
        return _xmlTransform.Transform(_xmlDoc);
    }

    [Benchmark(Description = "JSON deserialize")]
    public Expression Json_Deserialize()
    {
        return _jsonTransform.Transform(_jsonDoc);
    }
}

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
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class RoundTripBenchmarks
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

    ExpressionXmlTransform _xml = null!;
    ExpressionJsonTransform _json = null!;

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
        _xml = new ExpressionXmlTransform(new XmlOptions
        {
            Indent = false,
            AddComments = false,
            ValidateInputDocuments = ValidateInputDocuments,
        });

        _json = new ExpressionJsonTransform(new JsonOptions
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
        var xmlRoundTrip = _xml.Transform(_xmlDoc);
        if (!_expression.DeepEquals(xmlRoundTrip))
            throw new InvalidOperationException($"XML round-trip mismatch for case '{CaseName}'.");

        var jsonRoundTrip = _json.Transform(_jsonDoc);
        if (!_expression.DeepEquals(jsonRoundTrip))
            throw new InvalidOperationException($"JSON round-trip mismatch for case '{CaseName}'.");
    }

    [Benchmark(Description = "XML serialize")]
    public XDocument Xml_Serialize()
    {
        return _xml.Transform(_expression);
    }

    [Benchmark(Description = "JSON serialize")]
    public JsonObject Json_Serialize()
    {
        return _json.Transform(_expression);
    }

    [Benchmark(Description = "XML deserialize")]
    public Expression Xml_Deserialize()
    {
        return _xml.Transform(_xmlDoc);
    }

    [Benchmark(Description = "JSON deserialize")]
    public Expression Json_Deserialize()
    {
        return _json.Transform(_jsonDoc);
    }

    static Expression<Func<int, int, int>> BuildBlockExpression()
    {
        var local = Expression.Variable(typeof(int), "tmp");

        var block = Expression.Block(
            new[] { local },
            Expression.Assign(local, Expression.Add(_x, Expression.Constant(1))),
            Expression.Multiply(local, _y));

        return Expression.Lambda<Func<int, int, int>>(block, _x, _y);
    }
}

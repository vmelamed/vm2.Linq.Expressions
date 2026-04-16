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

    readonly ExpressionXmlTransform _xml = new(new XmlOptions
    {
        Indent = false,
        AddComments = false,
        ValidateInputDocuments = ValidateExpressionDocuments.Never,
    });

    readonly ExpressionJsonTransform _json = new(new JsonOptions
    {
        Indent = false,
        AddComments = false,
        ValidateInputDocuments = ValidateExpressionDocuments.Never,
    });

    Expression _expression = null!;

    [ParamsSource(nameof(CaseNames))]
    public string CaseName { get; set; } = string.Empty;

    public static IEnumerable<string> CaseNames => _cases.Keys;

    [GlobalSetup]
    public void Setup()
    {
        _expression = _cases[CaseName];

        // Guardrail: fail benchmark setup if round-trip correctness regresses.
        var xmlRoundTrip = _xml.Transform(_xml.Transform(_expression));
        if (!_expression.DeepEquals(xmlRoundTrip))
            throw new InvalidOperationException($"XML round-trip mismatch for case '{CaseName}'.");

        var jsonRoundTrip = _json.Transform(_json.Transform(_expression));
        if (!_expression.DeepEquals(jsonRoundTrip))
            throw new InvalidOperationException($"JSON round-trip mismatch for case '{CaseName}'.");
    }

    [Benchmark(Description = "XML round-trip")]
    public Expression Xml_RoundTrip()
    {
        var doc = _xml.Transform(_expression);
        return _xml.Transform(doc);
    }

    [Benchmark(Description = "JSON round-trip")]
    public Expression Json_RoundTrip()
    {
        var doc = _json.Transform(_expression);
        return _json.Transform(doc);
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

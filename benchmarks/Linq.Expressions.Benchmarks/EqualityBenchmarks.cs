// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Linq.Expressions.Benchmarks;

#pragma warning disable CA1822 // Mark members as static

#if SHORT_RUN
[ShortRunJob]
#else
[SimpleJob(RuntimeMoniker.HostProcess)]
#endif
[Orderer(SummaryOrderPolicy.Default)]
[JsonExporter]
[MarkdownExporter]
public class EqualityBenchmarks
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

    // the second expression is structurally identical but a different instance, to test that DeepEquals returns true. When the
    // same instance is used, the benchmark is dominated by ReferenceEquals and doesn't measure the actual node-by-node comparison.
    // (and yes, this is a bit of a hack, but it keeps the benchmark simple and focused on DeepEquals itself, without needing to
    // construct the expressions in the benchmark method).
    // The reason we use identical structure is to test the worst-case scenario for DeepEquals, where it should return true
    // traversing the entire tree, rather than short-circuiting on a difference.

    static readonly ParameterExpression _x2 = Expression.Parameter(typeof(int), "x");
    static readonly ParameterExpression _y2 = Expression.Parameter(typeof(int), "y");
    static readonly ParameterExpression _s2 = Expression.Parameter(typeof(string), "s");
    static readonly ParameterExpression _arr2 = Expression.Parameter(typeof(int[]), "arr");

    static readonly FrozenDictionary<string, Expression> _cases2 =
        new Dictionary<string, Expression>
        {
            ["Arithmetic"] = (Expression<Func<int, int, int>>)((x, y) => x * y + 2),
            ["Conditional"] = (Expression<Func<int, int>>)(x => x > 0 ? x : -x),
            ["MethodCall"] = (Expression<Func<string, string>>)(s => s.Trim().ToUpperInvariant()),
            ["LinqCall"] = (Expression<Func<int[], int>>)(arr => arr.Where(n => n > 0).Sum()),
            ["Block"] = BuildBlockExpression(),
        }.ToFrozenDictionary(StringComparer.Ordinal);

    Expression _expression = null!;
    Expression _expression2 = null!;

    [ParamsSource(nameof(CaseNames))]
    public string CaseName { get; set; } = string.Empty;

    public static IEnumerable<string> CaseNames => _cases.Keys;

    [GlobalSetup]
    public void Setup()
    {
        _expression = _cases[CaseName];
        _expression2 = _cases2[CaseName];
    }

    [Benchmark(Description = "DeepEquals")]
    public bool DeepEquals()
    {
        return _expression.DeepEquals(_expression2);
    }

    [Benchmark(Description = "GetDeepHashCode")]
    public int GetDeepHashCode()
    {
        return _expression.GetDeepHashCode();
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

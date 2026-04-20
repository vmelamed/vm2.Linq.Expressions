# The Timeless Marriage: GoF Visitor, LINQ Expression Trees, and Why 30-Year-Old Patterns Still Ship Code

<!-- TOC tocDepth:2..3 chapterDepth:2..6 -->

- [The Timeless Marriage: GoF Visitor, LINQ Expression Trees, and Why 30-Year-Old Patterns Still Ship Code](#the-timeless-marriage-gof-visitor-linq-expression-trees-and-why-30-year-old-patterns-still-ship-code)
  - [Patterns Don't Die — They Get Promoted to the Standard Library](#patterns-dont-die--they-get-promoted-to-the-standard-library)
  - [LINQ: Still Beautiful After All These Years](#linq-still-beautiful-after-all-these-years)
  - [The Marriage: How LINQ Queries Actually Work](#the-marriage-how-linq-queries-actually-work)
  - [What Else Can This Marriage Do?](#what-else-can-this-marriage-do)
  - [HashCodeVisitor: The Simplest Visitor You'll Ever Write](#hashcodevisitor-the-simplest-visitor-youll-ever-write)
  - [DeepEqualsComparer: The Kitchen Fight that Copilot Won](#deepequalscomparer-the-kitchen-fight-that-copilot-won)
  - [The Serialization Visitors: Where the Boring Details Live](#the-serialization-visitors-where-the-boring-details-live)
  - [A Word on Security (Yes, Again)](#a-word-on-security-yes-again)
  - [Performance: XML vs JSON, and the Validation Tax](#performance-xml-vs-json-and-the-validation-tax)
  - [Document Validation: Two Implementations, One Honest Recommendation](#document-validation-two-implementations-one-honest-recommendation)
  - [Conclusion: A Timeless Match](#conclusion-a-timeless-match)
    - [Genuinely Threatened by AI](#genuinely-threatened-by-ai)
    - [Still Rock-Solid — AI Can't Touch These](#still-rock-solid--ai-cant-touch-these)
    - [The New Category — and This Is the One That Should Make You Think](#the-new-category--and-this-is-the-one-that-should-make-you-think)
  - [References](#references)

<!-- /TOC -->

## Patterns Don't Die — They Get Promoted to the Standard Library

In 1994, the *Gang of Four* published *Design Patterns: Elements of reusable Object-Oriented Software*. More than thirty years later, those patterns are not textbook exercises gathering dust on a shelf — they are load-bearing infrastructure in the most widely used frameworks on the planet. If you write C# and use LINQ, you use the Visitor pattern every day whether you know it or not.

If you're thinking "who cares about patterns when we have AI?" — skip to the [Conclusion](#conclusion-a-timeless-match) first. If it intrigues you, come back and read the whole thing.

The Visitor is one of my favorite patterns. The premise is simple: you have a hierarchy of types that is closed (you can't or don't want to modify them), but you need to keep adding new *operations* over those types. The Visitor lets you do exactly that. Instead of scattering new methods across every class in the hierarchy, you write a single new class — a visitor — that handles every node type. New operation? New visitor. The original types remain untouched.

This is the Open/Closed Principle made real: open for extension, closed for modification. Not in a slide deck. In production.

## LINQ: Still Beautiful After All These Years

More than fifteen years after its introduction, LINQ remains one of the most elegant features of .NET. Most developers use it daily for querying collections:

```csharp
var results = orders
    .Where(o => o.Total > 100)
    .OrderBy(o => o.Date)
    .Select(o => new { o.Id, o.Total });
```

But LINQ has a second life that many developers never see. When you write a lambda and assign it to an `Expression<TDelegate>` instead of a bare `Func<>` or `Action<>`, the compiler doesn't compile it into IL. It compiles it into a *data structure* — an expression tree. An Abstract Syntax Tree (AST).

```csharp
// This is compiled code — a delegate:
Func<int, int, int> fn = (x, y) => x * y + 42;

// This is data — an AST you can inspect, transform, and serialize:
Expression<Func<int, int, int>> expr = (x, y) => x * y + 42;
```

ASTs are one of the oldest ideas in computer science. Every compiler builds one. Roslyn builds one for your C# code. What makes LINQ expression trees special is that Microsoft put an AST representation *into the standard library* and gave it first-class language support. That's the beautiful part.

Here's what the expression tree for `(x, y) => x * y + 42` actually looks like in memory:

![Image description](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/dfvr52lmvbny5ej34d81.png)

Every box (except the `Visitor`) is a .NET object — a subclass of `Expression`. The edges are properties (`Body`, `Left`, `Right`, `Parameters[]`). The Visitor walks this tree top-down, dispatching to a type-specific `Visit*` method at each node. That's the structure that makes everything in this post possible.

## The Marriage: How LINQ Queries Actually Work

Here's the payoff: this is exactly how LINQ providers work. When Entity Framework translates your C# query into SQL, it doesn't execute your lambda. It receives the expression tree, walks the AST, and *translates* it — node by node — into a different language.

That traversal? It's the Visitor pattern. `System.Linq.Expressions.ExpressionVisitor` is a textbook GoF Visitor, right there in the BCL. Every LINQ provider that targets a database, a search index, a REST API, or a CSV file uses this same mechanism. The expression tree is the universal intermediate representation; the visitor is the universal translator.

This is how one C# syntax — `Where`, `Select`, `OrderBy` — queries everything from in-memory lists to SQL Server to Cosmos DB and MongoDB. The AST and the Visitor are the engine underneath.

## What Else Can This Marriage Do?

Querying databases is the headline use case, but the same architecture applies whenever you need to *do things* with expression trees beyond compiling and executing them. Here are three:

1. **Structural deep equality** — are two expression trees identical in structure and content?
2. **Structural hash codes** — can we put expression trees in hash sets, use them as dictionary keys, or deduplicate them?
3. **Serialization** — can we persist an expression tree to XML or JSON, transmit it across a network, and reconstruct it on the other side?

These are the problems that [`vm2.Linq.Expressions`](https://github.com/vmelamed/vm2.Linq.Expressions) solves. And the approach is exactly what you'd expect: visitors. Let me walk you through them, starting with the simplest.

## HashCodeVisitor: The Simplest Visitor You'll Ever Write

The `HashCodeVisitor` computes a structural hash code for any expression tree. Here's the essence:

```csharp
public sealed class HashCodeVisitor : ExpressionVisitor
{
    HashCode _hc = new();

    public override Expression? Visit(Expression? node)
    {
        if (node is null) return null;

        _hc.Add(node.NodeType);
        _hc.Add(node.Type.GetHashCode());

        return base.Visit(node); // ← the base handles all child traversal
    }

    public int ToHashCode() => _hc.ToHashCode();

    protected override Expression VisitConstant(ConstantExpression node)
    {
        _hc.Add(node.Value is null);
        if (node.Value is not null)
            _hc.Add(node.Value);
        return base.VisitConstant(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _hc.Add(node.IsLifted);
        _hc.Add(node.IsLiftedToNull);
        VisitMemberInfo(node.Method);
        return base.VisitBinary(node);
    }

   // similar 2-3 line overrides for each node type with scalar properties
}
```

That's it. The key insight: you only need to override the node types that have *scalar* properties (constants, method references, lifted flags, etc.). The base `ExpressionVisitor` handles all child traversal automatically — it knows how to walk into a `BinaryExpression`'s `Left`, `Right`, and `Conversion` children, into a `LambdaExpression`'s `Body` and `Parameters`, and so on. You just add the non-structural data to the hash at each node.

Usage:

```csharp
Expression<Func<int, int, int>> e1 = (x, y) => x * y + 42;
Expression<Func<int, int, int>> e2 = (x, y) => x * y + 42;

e1.GetDeepHashCode() == e2.GetDeepHashCode()  // true — same structure, same hash
```

Two independently compiled expression trees, created at different times, produce the same hash because they have the same AST structure. That's structural identity, and it takes about 120 lines of code thanks to the Visitor doing the heavy lifting.

## DeepEqualsComparer: The Kitchen Fight that Copilot Won

Now here's where things got personal.

The old implementation of deep equality used the Visitor pattern — twice. An `EnqueueingVisitor` walked the right-hand expression tree and pushed every node into a `Queue<object?>`. Then a `DeepEqualsVisitor` walked the left-hand tree and dequeued nodes one by one, comparing them in lockstep. Two visitors, one implicit contract: they had to visit nodes in *exactly the same order*. No compiler checked this. No test could easily catch a mismatch. If you added a new node type and the two visitors visited children in a slightly different sequence, they would silently de-synchronize and produce wrong results.

It worked. It had been working for years. And its author — yours truly — was rather attached to it.

Enter the Copilot. "This two-class design is fragile," it said. "You have an implicit ordering contract between two independent traversals. One walks, one queues. If they ever disagree on visit order, you get silent wrong results. Let me rewrite it as a single recursive walker that traverses both trees in lockstep."

"But... But... But it *works*," I protested and the child order visiting is guaranteed by the BCL, and...

"It works *today*. Add a node type tomorrow, get the child order slightly wrong in one of the two visitors, and you'll spend a weekend debugging a false `true`."

"I've never had that bug."

"You've never had that bug *yet*. Also, why are you allocating a queue? You're turning a tree walk into a queue-drain. That's a cathedral where a tent would do."

"Did you just call my code a cathedral?"

"I called your code *over-engineered*. Look — one class, explicit recursion, both trees in the same code path:"

```csharp
class DeepEqualsComparer
{
    public bool Compare(Expression? left, Expression? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return Fail(left, right);
        if (left.Type != right.Type || left.NodeType != right.NodeType)
            return Fail(left, right);

        return (left, right) switch
        {
            (BinaryExpression l, BinaryExpression r)     => CompareBinary(l, r),
            (ConstantExpression l, ConstantExpression r) => CompareConstant(l, r),
            (LambdaExpression l, LambdaExpression r)     => CompareLambda(l, r),
            // ... every node type
            _ => true,
        };
    }

    bool CompareBinary(BinaryExpression left, BinaryExpression right)
    {
        if (left.IsLifted != right.IsLifted || left.Method != right.Method)
            return Fail(left, right);

        return Compare(left.Left, right.Left)
            && Compare(left.Right, right.Right)
            && Compare(left.Conversion, right.Conversion);
    }
}
```

No queue. No two-class contract. No possibility of traversal de-synchronization. One class walks both trees, and `CompareConstant` handles the gnarly reflection cases — `Nullable<T>`, `Memory<T>`, `ReadOnlyMemory<T>`, `ArraySegment<T>` — all in one place.

I stared at it. I tried to find a flaw. I muttered something about "but the Visitor pattern and the OCP..." and the AI — politely but firmly — pointed out that a hand-rolled recursive descent walker that dispatches on node types *is* a visitor and OCP is not broken either. It just doesn't inherit from `ExpressionVisitor`. The *intent* is identical. The *implementation* is cleaner.

Fine. The AI won this one. I'll admit it — grudgingly, and only in writing, and only because you're reading this and I can't take it back now.

Seriously though, sometimes the best way to honor a pattern is to know when its base class infrastructure doesn't quite fit your problem. (And sometimes it takes a machine with no ego to tell you that.)

## The Serialization Visitors: Where the Boring Details Live

The XML and JSON serializers *are* proper `ExpressionVisitor` subclasses. Both extend `ExpressionTransformVisitor<TElement>`, a base class that adds a stack-based, reverse-Polish-notation approach: each `Visit*` override pushes document elements onto a stack, and the parent node pops its children when constructing its own output.

```csharp
public abstract class ExpressionTransformVisitor<TElement> : ExpressionVisitor
{
    protected readonly Stack<TElement> _elements = new();

    // GenericVisit: call base, create empty node, populate it, push result
    protected virtual Expression GenericVisit<TExpression>(
        TExpression node,
        Func<TExpression, Expression> baseVisit,
        Action<TExpression, TElement> thisVisit) { ... }
}
```

`ToXmlTransformVisitor` produces `XElement` trees. `ToJsonTransformVisitor` produces `JsonObject` graphs. Each has a
corresponding `From*TransformVisitor` that reads the document back into expression trees.

I won't walk through the gory details of every `Visit*` override — there are about 25 of them in each direction, covering every expression node
type from `BinaryExpression` to `TryExpression` to `SwitchCase`. And there is too much reflection code for my taste. The implementation is thorough, methodical, and — let's be honest — a lot of boring plumbing. Type serialization, member metadata, parameter/label identity tracking, constant handling for every BCL type including `Memory<T>`, `Nullable<T>`, even `Frozen*<>`, and generic collections.

You may well implement parts of it better. The important point isn't the plumbing — it's that the Visitor pattern makes the architecture *possible*. Four visitors, four entirely different operations, zero modifications to any `Expression` subclass (which you couldn't modify anyway — they're sealed BCL types). That's the pattern paying for itself.

## A Word on Security (Yes, Again)

Deserializing a LINQ expression tree is deserializing *executable code*. The resulting `Expression` can be and probably will be compiled into a delegate and invoked. This makes expression documents a potential vector for remote code execution if an attacker can supply or tamper with the input. In some cases, some constants may reveal sensitive data.

The risks are real:

- **Arbitrary type instantiation** — the document contains assembly-qualified type names resolved via reflection.
- **Method invocation** — `MethodCall` nodes can describe calls to *any* accessible method, including `Process.Start`, file I/O, network access.
- **Constant injection** — `Constant` nodes can carry arbitrary literal values, including connection strings or credentials.

The mitigations are the same ones you'd apply to any code-loading scenario: treat serialized expressions as untrusted code, wrap them in a signed envelope (JWS, XML Digital Signature), attach security metadata (allow-listed types, maximum depth, authorized principals), and restrict the type universe after deserialization by walking the tree before compiling.

This isn't new advice. But it bears repeating every time someone builds a new serialization format, because the next developer to pick up this library needs to hear it *before* they pipe untrusted JSON into `ExpressionJson.FromString()`.

## Performance: XML vs JSON, and the Validation Tax

Both formats perform similarly for serialization and un-validated deserialization — in the low-microsecond range:

| Operation | Format | Validation | Typical (simple expr) |
|---|---|---|---|
| Serialize | XML | N/A | ~2.3 μs |
| Serialize | JSON | N/A | ~4.5 μs |
| Deserialize | XML | Never | ~1.7 μs |
| Deserialize | JSON | Never | ~1.8 μs |
| Deserialize | XML | Always | ~6.2 μs |
| **Deserialize** | **JSON** | **Always** | **~2,500 μs** |

Just look at the last row. JSON deserialization with schema validation is roughly **1,000x slower** than without, and about **400x slower** than XML validation. XML validation (using the built-in `System.Xml.Schema`) adds a moderate 3-4x overhead. JSON Schema validation (using `JsonSchema.Net`) adds **orders of magnitude**.

Why? XML schema validation is tightly integrated into the .NET XML stack — it's a streaming, single-pass operation. JSON Schema is a specification designed around a different set of trade-offs; the .NET implementations evaluate it in a way that involves re-parsing, allocation-heavy evaluation, and complex `$ref` / `oneOf` resolution. The `Block` case with validation allocates **5.4 MB** per operation versus **2.6 KB** without.

## Document Validation: Two Implementations, One Honest Recommendation

The library ships with both XML schemas (XSD) and a JSON Schema. For XML, validation just works — `System.Xml.Schema` handles it natively with no issues. For JSON, the picture is more nuanced.

The default JSON validation uses [JsonSchema.Net](https://github.com/gregsdennis/json-everything) — a capable, free, actively maintained library. It handles the vast majority of expression patterns correctly. But the expression schema is complex: deeply nested recursive `$ref` combined with `oneOf` constructs push the validator to its limits. Currently, 2 out of 718 tested expression patterns produce false validation failures — documents that are structurally correct but trip the validator. The author of JsonSchema.Net has done genuinely excellent work; this is an edge case in a very complex schema, not a quality problem.

If you need *complete* JSON validation fidelity, the library also supports building with [Newtonsoft.Json.Schema](https://www.newtonsoft.com/jsonschema) via the `NEWTONSOFT_SCHEMA` preprocessor symbol. NSJ handles every test case correctly — but it's commercially licensed (free tier: 1,000 validations/hour).

The practical recommendation: **for most production scenarios, turn validation off.** If both the producer and consumer of a serialized expression use the same version of the library, the output is guaranteed to conform to the schema. Validation becomes valuable when documents cross trust boundaries, are stored long-term, or are generated by third-party tools. And when you do need it on hot paths, prefer XML.

## Conclusion: A Timeless Match

The LINQ expression tree — a language-integrated AST in the standard library — and the Visitor pattern — a 30-year-old design from the GoF book — are a timeless match. Microsoft chose this combination in 2007, and it's still the engine underneath every LINQ provider, every ORM, every rule engine that operates on expression trees.

The `vm2.Linq.Expressions` packages extend that engine with structural deep equality, hash codes, and XML/JSON serialization. The use cases are real and span decades of production software. But let's be honest about which ones still matter in 2026.

### Genuinely Threatened by AI

Let's not kid ourselves — some traditional uses of predicate trees are losing ground fast:

- **Ad-hoc query builders** — "drag and drop your filter" is losing to "just ask in plain English." Text-to-SQL is good enough for most exploratory queries now. If the user never sees the predicate, why serialize it?
- **Feature targeting rules** — hand-crafted "region == EU AND tier >= Pro" is giving way to ML-driven personalization that figures out targeting on its own.
- **ETL transformations** — AI can infer column mappings from examples. The manual rule-composition UI? Fading.

Fair enough. But here's where it gets interesting.

### Still Rock-Solid — AI Can't Touch These

Anywhere the law, security policy, or patient safety requires *deterministic, inspect-able, version-able rules with a full audit trail*, expression trees are as relevant as ever — arguably more so, because the alternative (opaque ML) is explicitly prohibited in many jurisdictions:

- **Regulatory compliance** — Basel III, SOX, MiFID II, FDA. Try telling a regulator "the model decided." The rule *is* the audit trail. Version it, diff it, prove what was in effect on a given date.
- **Security policy enforcement** — ABAC, zero-trust, SIEM alert rules. You really, truly do not want an LLM making access control decisions at runtime. Policies must be exact, reproducible, and fast.
- **Clinical trial criteria** — FDA and EMA require exact, reproducible inclusion/exclusion predicates. AI might help *find* matching patients, but the criteria themselves must be formal and auditable.

I've built systems in two of these categories personally — portfolio selection rules for investment fund managers (more than 25 years ago, in C++) and security policy engines for SysAdmins selecting groups of resources and applying rules to them. The technology changed. The fundamental need didn't.

### The New Category — and This Is the One That Should Make You Think

Here's the twist I didn't see coming: AI doesn't replace expression trees. It creates *more demand for them*.

Think about it. An LLM suggests a pricing rule, a compliance check, a data filter. Great — but that suggestion is an opaque string. Now materialize it as an expression tree, and suddenly you can **inspect** it, **diff** it against yesterday's version, **test** it, **serialize** it for audit, and **enforce** it deterministically. The AI proposes; the expression tree is the verifiable artifact.

- "Is this the same rule the model generated yesterday?" — that's `DeepEquals`.
- "Can we cache and deduplicate these generated predicates?" — that's `GetDeepHashCode`.
- "Can we store the rule for regulatory review?" — that's serialization.
- "The model says approve this trade, but the compliance predicate says the counterparty is sanctioned" — that's a deterministic expression tree acting as a **guardrail** over AI-generated logic.

The punchline: the more AI-generated logic enters your system, the more you need a deterministic, inspect-able, diff-able, serializable representation of that logic. Expression trees are exactly that.

---

The patterns, the AST, and the Visitor will outlive the current AI hype cycle — and the next one. Not because they're fashionable (they never were, really), but because they solve problems that don't go away: traversal, extension, separation of concerns, verifiability. And in a world increasingly full of AI-generated logic, having a structured, typed, comparable representation of "what the machine decided" isn't a luxury. It's a requirement.

---

## References

- Gamma, E., Helm, R., Johnson, R., & Vlissides, J. (1994). *Design Patterns: Elements of Reusable Object-Oriented Software*. Addison-Wesley.
- Microsoft. [Expression Trees (C#)](https://learn.microsoft.com/dotnet/csharp/advanced-topics/expression-trees/). Microsoft Learn.
- Microsoft. [System.Linq.Expressions.ExpressionVisitor](https://learn.microsoft.com/dotnet/api/system.linq.expressions.expressionvisitor). .NET API Reference.
- Microsoft. [LINQ Providers](https://learn.microsoft.com/dotnet/csharp/linq/). Microsoft Learn.
- Denning, G. [JsonSchema.Net](https://github.com/gregsdennis/json-everything). GitHub.
- Newton-King, J. [Newtonsoft.Json.Schema](https://www.newtonsoft.com/jsonschema). Newtonsoft.
- [vm2.Linq.Expressions](https://github.com/vmelamed/vm2.Linq.Expressions). GitHub.

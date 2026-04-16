namespace vm2.Linq.Expressions.Serialization.Xml.Tests;

public class XmlFacadeTests
{
    static readonly Expression<Func<int, int, int>> _expr = (x, y) => x * y + 2;
    static readonly Expression<Func<int[], int>> _linqExpr = arr => arr.Where(n => n > 0).Sum();
    static readonly Expression<Func<int[], int>> _genericArrayExpr = arr => GenericMethodFixtures.CountArray(arr);
    static readonly Expression<Func<DerivedIntList, int>> _genericListExpr = list => GenericMethodFixtures.CountList(list);
    static readonly Expression<Func<int[], int>> _genericConstrainedExpr = arr => GenericMethodFixtures.CountStructs(arr);
    static readonly Expression<Func<int>> _genericByRefExpr = BuildGenericByRefExpr();

    static Expression<Func<int>> BuildGenericByRefExpr()
    {
        var v = Expression.Variable(typeof(int), "v");
        var method = typeof(GenericMethodFixtures)
            .GetMethod(nameof(GenericMethodFixtures.EchoByRef))!
            .MakeGenericMethod(typeof(int));

        return Expression.Lambda<Func<int>>(
            Expression.Block(
                [v],
                Expression.Assign(v, Expression.Constant(7)),
                Expression.Call(method, v)));
    }

    static Type? InvokeGetMatchingGenericType(Type actualType, Type genericTypeDefinition)
    {
        var visitorType = Type.GetType("vm2.Linq.Expressions.Serialization.Xml.FromXmlTransformVisitor, Serialization.Xml", throwOnError: true)!;
        var method = visitorType.GetMethod("GetMatchingGenericType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
        return (Type?)method.Invoke(null, [actualType, genericTypeDefinition]);
    }

    static bool InvokeParametersMatch(System.Reflection.ParameterInfo[] formalParams, Type[] actualTypes)
    {
        var visitorType = Type.GetType("vm2.Linq.Expressions.Serialization.Xml.FromXmlTransformVisitor, Serialization.Xml", throwOnError: true)!;
        var method = visitorType.GetMethod("ParametersMatch", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
        return (bool)method.Invoke(null, [formalParams, actualTypes])!;
    }

    static bool InvokeTryCloseGenericMethod(System.Reflection.MethodInfo inputMethod, Type[] parameterTypes, out System.Reflection.MethodInfo candidate)
    {
        var visitorType = Type.GetType("vm2.Linq.Expressions.Serialization.Xml.FromXmlTransformVisitor, Serialization.Xml", throwOnError: true)!;
        var method = visitorType.GetMethod("TryCloseGenericMethod", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
        object?[] args = [inputMethod, parameterTypes, null];
        var result = (bool)method.Invoke(null, args)!;
        candidate = (System.Reflection.MethodInfo)args[2]!;
        return result;
    }

    sealed class NonClosingMemoryStream : MemoryStream
    {
        protected override void Dispose(bool disposing)
        {
            // Keep the underlying memory stream readable after facade methods dispose nested writers.
            Flush();
        }
    }

    [Fact]
    public void ToXmlDocument_And_ToExpression_RoundTrip()
    {
        var doc = _expr.ToXmlDocument();

        var roundTrip = doc.ToExpression();

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToXmlString_And_FromString_RoundTrip()
    {
        var xml = _expr.ToXmlString();

        var roundTrip = ExpressionXml.FromString(xml);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToXmlStream_And_FromStream_RoundTrip()
    {
        using var stream = new NonClosingMemoryStream();
        _expr.ToXmlStream(stream);

        stream.Position = 0;
        var roundTrip = ExpressionXml.FromStream(stream);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToXmlWriter_And_FromReader_RoundTrip()
    {
        using var sw = new StringWriter();
        using (var writer = XmlWriter.Create(sw))
            _expr.ToXmlWriter(writer);

        using var sr = new StringReader(sw.ToString());
        using var reader = XmlReader.Create(sr);
        var roundTrip = ExpressionXml.FromReader(reader);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToXmlFile_And_FromFile_RoundTrip()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"xml-facade-{Guid.NewGuid():N}.xml");

        try
        {
            _expr.ToXmlFile(filePath);
            var roundTrip = ExpressionXml.FromFile(filePath);

            _expr.DeepEquals(roundTrip).Should().BeTrue();
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task AsyncFacadeMethods_RoundTrip()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        using var stream = new NonClosingMemoryStream();
        await _expr.ToXmlStreamAsync(stream, cancellationToken: cancellationToken);

        stream.Position = 0;
        var fromStream = await ExpressionXml.FromStreamAsync(stream, cancellationToken: cancellationToken);
        _expr.DeepEquals(fromStream).Should().BeTrue();

        var filePath = Path.Combine(Path.GetTempPath(), $"xml-facade-async-{Guid.NewGuid():N}.xml");

        try
        {
            await _expr.ToXmlFileAsync(filePath, cancellationToken: cancellationToken);
            var fromFile = await ExpressionXml.FromFileAsync(filePath, cancellationToken: cancellationToken);
            _expr.DeepEquals(fromFile).Should().BeTrue();
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        using var sw = new StringWriter();
        var settings = new XmlWriterSettings { Async = true, OmitXmlDeclaration = false };
        using (var writer = XmlWriter.Create(sw, settings))
            await _expr.ToXmlWriterAsync(writer, cancellationToken: cancellationToken);

        using var sr = new StringReader(sw.ToString());
        using var reader = XmlReader.Create(sr, new XmlReaderSettings { Async = true });
        var fromReader = await ExpressionXml.FromReaderAsync(reader, cancellationToken: cancellationToken);
        _expr.DeepEquals(fromReader).Should().BeTrue();
    }

    [Fact]
    public void ToXmlString_RespectsOptions()
    {
        var options = new XmlOptions
        {
            AddComments = true,
            Indent = true,
        };

        var xml = _expr.ToXmlString(options);

        xml.Should().Contain("<!--");
    }

    [Fact]
    public void ToXmlDocument_And_ToExpression_RoundTrip_LinqCall()
    {
        var doc = _linqExpr.ToXmlDocument();

        var roundTrip = doc.ToExpression();

        _linqExpr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToXmlDocument_And_ToExpression_RoundTrip_GenericArrayCall()
    {
        var doc = _genericArrayExpr.ToXmlDocument();

        var roundTrip = doc.ToExpression();

        _genericArrayExpr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToXmlDocument_And_ToExpression_RoundTrip_GenericListCall()
    {
        var doc = _genericListExpr.ToXmlDocument();

        var roundTrip = doc.ToExpression();

        _genericListExpr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToXmlDocument_And_ToExpression_RoundTrip_GenericByRefCall()
    {
        var doc = _genericByRefExpr.ToXmlDocument();

        var roundTrip = doc.ToExpression();

        _genericByRefExpr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void Resolver_GetMatchingGenericType_MatchesImplementedInterface()
    {
        var match = InvokeGetMatchingGenericType(typeof(int[]), typeof(IEnumerable<>));

        match.Should().NotBeNull();
        match!.GetGenericTypeDefinition().Should().Be(typeof(IEnumerable<>));
    }

    [Fact]
    public void Resolver_GetMatchingGenericType_MatchesGenericBaseType()
    {
        var match = InvokeGetMatchingGenericType(typeof(DerivedIntList), typeof(List<>));

        match.Should().Be(typeof(List<int>));
    }

    [Fact]
    public void Resolver_GetMatchingGenericType_ReturnsNull_WhenNoMatch()
    {
        var match = InvokeGetMatchingGenericType(typeof(DateTime), typeof(IEnumerable<>));

        match.Should().BeNull();
    }

    [Fact]
    public void Resolver_ParametersMatch_ReturnsFalse_ForLengthMismatch()
    {
        var formal = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!.GetParameters();

        var match = InvokeParametersMatch(formal, [typeof(string), typeof(StringComparison)]);

        match.Should().BeFalse();
    }

    [Fact]
    public void Resolver_ParametersMatch_ReturnsFalse_ForTypeMismatch()
    {
        var formal = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!.GetParameters();

        var match = InvokeParametersMatch(formal, [typeof(int)]);

        match.Should().BeFalse();
    }

    [Fact]
    public void Resolver_TryCloseGenericMethod_ReturnsFalse_ForConstraintViolation()
    {
        var method = typeof(GenericMethodFixtures).GetMethod(nameof(GenericMethodFixtures.CountStructs))!;

        var result = InvokeTryCloseGenericMethod(method, [typeof(IEnumerable<string>)], out var candidate);

        result.Should().BeFalse();
        candidate.Should().BeSameAs(method);
    }

    [Fact]
    public void Resolver_TryCloseGenericMethod_ReturnsFalse_ForNonGenericMismatch()
    {
        var method = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!;

        var result = InvokeTryCloseGenericMethod(method, [typeof(int)], out _);

        result.Should().BeFalse();
    }

    [Fact]
    public void ToExpression_ThrowsSerializationException_WhenMethodNameIsInvalid()
    {
        var doc = _linqExpr.ToXmlDocument();
        var method = doc.Descendants().Single(
            n => n.Name.LocalName == "method"
                 && (string?)n.Attribute("name") == "Where");

        method.SetAttributeValue("name", "Where_NoSuchOverload");

        var act = () => doc.ToExpression();

        act.Should().Throw<SerializationException>();
    }

    [Fact]
    public void ToExpression_ThrowsSerializationException_WhenGenericConstraintsDoNotMatch()
    {
        var doc = _genericConstrainedExpr.ToXmlDocument();
        var method = doc.Descendants().Single(
            n => n.Name.LocalName == "method"
                 && (string?)n.Attribute("name") == "CountStructs");
        var itemsSpec = method
            .Descendants()
            .Single(
                n => n.Name.LocalName == "parameterSpec"
                     && (string?)n.Attribute("name") == "items");

        itemsSpec.SetAttributeValue("type", typeof(IEnumerable<string>).AssemblyQualifiedName);

        var act = () => doc.ToExpression();

        act.Should().Throw<SerializationException>();
    }
}

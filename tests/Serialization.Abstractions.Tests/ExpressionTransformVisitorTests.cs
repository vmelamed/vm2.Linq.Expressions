// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Abstractions;

using System.Linq.Expressions;

public class ExpressionTransformVisitorTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void Result_WhenStackHasMoreThanOneElement_ShouldThrowInternalTransformErrorException()
    {
        var visitor = new TestVisitor();
        visitor.Push("one");
        visitor.Push("two");

        var call = () => visitor.Result;

        call.Should().Throw<InternalTransformErrorException>();
    }

    [Fact]
    public void Result_WhenStackIsEmpty_ShouldThrowNoAvailableResultException()
    {
        var visitor = new TestVisitor();

        var call = () => visitor.Result;

        call.Should().Throw<NoAvailableResultException>();
    }

    [Fact]
    public void Result_WhenStackHasExactlyOneElement_ShouldReturnElementAndResetVisitor()
    {
        var visitor = new TestVisitor();
        visitor.Push("root");

        var result = visitor.Result;

        result.Should().Be("root");
        visitor.Count.Should().Be(0);
    }

    [Fact]
    public void Pop_WhenCalled_ShouldReturnTopElement()
    {
        var visitor = new TestVisitor();
        visitor.Push("first");
        visitor.Push("second");

        var result = visitor.PopOnePublic();

        result.Should().Be("second");
        visitor.Count.Should().Be(1);
    }

    [Fact]
    public void Pop_WhenPoppingMultipleElements_ShouldReturnElementsInOriginalPushOrder()
    {
        var visitor = new TestVisitor();
        visitor.Push("a");
        visitor.Push("b");
        visitor.Push("c");

        var popped = visitor.PopManyPublic(2);

        popped.Should().Equal("b", "c");
        visitor.Count.Should().Be(1);
    }

    [Fact]
    public void GenericVisit_WhenBaseVisitReturnsNull_ShouldThrowInternalTransformErrorException()
    {
        var visitor = new TestVisitor();
        var node = Expression.Constant(123);

        var call = () => visitor.GenericVisitPublic(
            node,
            _ => null!,
            (_, _) => { });

        call.Should().Throw<InternalTransformErrorException>();
    }

    [Fact]
    public void GenericVisit_WhenBaseVisitReturnsDifferentExpressionType_ShouldReturnReducedNodeWithoutPushingElement()
    {
        var visitor = new TestVisitor();
        var node = Expression.Constant(123);
        var reduced = Expression.Add(Expression.Constant(1), Expression.Constant(2));
        var called = false;

        var result = visitor.GenericVisitPublic(
            node,
            _ => reduced,
            (_, _) => called = true);

        result.Should().BeSameAs(reduced);
        called.Should().BeFalse();
        visitor.Count.Should().Be(0);
    }

    [Fact]
    public void GenericVisit_WhenBaseVisitReturnsSameExpressionType_ShouldInvokeDelegateAndPushElement()
    {
        var visitor = new TestVisitor();
        var node = Expression.Constant(123);
        var called = false;
        var received = "";

        var result = visitor.GenericVisitPublic(
            node,
            n => n,
            (_, x) =>
            {
                called = true;
                received = x;
            });

        result.Should().BeSameAs(node);
        called.Should().BeTrue();
        received.Should().Be("<Constant>");
        visitor.Result.Should().Be("<Constant>");
    }

    [Fact]
    public void Visit_WhenNodeIsNull_ShouldReturnNull()
    {
        var visitor = new TestVisitor();

        var result = visitor.Visit((Expression?)null);

        result.Should().BeNull();
    }

    [Fact]
    public void Visit_WhenNodeIsNotNull_ShouldReturnVisitedExpression()
    {
        var visitor = new TestVisitor();
        var node = Expression.Constant(123);

        var result = visitor.Visit(node);

        result.Should().BeSameAs(node);
    }

    private sealed class TestVisitor : ExpressionTransformVisitor<string>
    {
        public int Count => _elements.Count;

        public void Push(string value) => _elements.Push(value);

        public string PopOnePublic() => Pop();

        public IReadOnlyList<string> PopManyPublic(int count) => Pop(count).ToList();

        public Expression GenericVisitPublic<TExpression>(
            TExpression node,
            Func<TExpression, Expression> baseVisit,
            Action<TExpression, string> thisVisit)
            where TExpression : Expression
            => GenericVisit(node, baseVisit, thisVisit);

        protected override string GetEmptyNode(Expression node) => $"<{node.NodeType}>";
    }
}

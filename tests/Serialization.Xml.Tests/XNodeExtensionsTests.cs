// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Xml;

public class XNodeExtensionsTests
{
    static readonly XName _xsiNil = XNamespace.Get(XmlOptions.Xsi) + "nil";

    public static readonly TheoryData<string, bool, string?, bool> NilData = new()
    {
        { TestLine(), false, null, false },
        { TestLine(), true,  "true", true },
        { TestLine(), true,  "false", false },
    };

    [Theory]
    [MemberData(nameof(NilData))]
    public void IsNil_ShouldFollowXsiNilAttribute(string _, bool addAttribute, string? value, bool expected)
    {
        var e = new XElement(XNamespace.Get(XmlOptions.Exs) + "constant");

        if (addAttribute)
            e.SetAttributeValue(_xsiNil, value);

        e.IsNil().Should().Be(expected);
    }

    public static readonly TheoryData<string, bool, bool, int, bool, bool> ChildData = new()
    {
        { TestLine(), true,  false, 0, true,  false },
        { TestLine(), true,  false, 2, false, true  },
        { TestLine(), false, true,  0, true,  false },
        { TestLine(), false, true,  2, false, true  },
    };

    [Theory]
    [MemberData(nameof(ChildData))]
    public void ChildAccessors_ShouldHandleFoundAndMissingChildren(string _, bool byIndex, bool byName, int index, bool shouldFind, bool shouldThrow)
    {
        var root = new XElement(XNamespace.Get(XmlOptions.Exs) + "root",
            new XElement(XNamespace.Get(XmlOptions.Exs) + "a"),
            new XElement(XNamespace.Get(XmlOptions.Exs) + "b"));

        if (byIndex)
        {
            var found = root.TryGetChild(index, out var child);

            found.Should().Be(shouldFind);
            if (shouldFind)
                child.Should().NotBeNull();

            Action act = () => root.GetChild(index);
            if (shouldThrow)
                act.Should().Throw<SerializationException>();
            else
                act.Should().NotThrow();
        }

        if (byName)
        {
            var name = shouldFind ? "a" : "missing";
            var found = root.TryGetChild(name, out var child);

            found.Should().Be(shouldFind);
            if (shouldFind)
                child.Should().NotBeNull();

            Action act = () => root.GetChild(name);
            if (shouldThrow)
                act.Should().Throw<SerializationException>();
            else
                act.Should().NotThrow();
        }
    }

    [Fact]
    public void TypeAndNameAccessors_ShouldUseAttributesThenElementFallbacks()
    {
        var fromAttribute = new XElement(XNamespace.Get(XmlOptions.Exs) + "constant");
        fromAttribute.SetAttributeValue("name", "x");
        fromAttribute.SetAttributeValue("length", "4");
        fromAttribute.SetAttributeValue("type", "int");

        fromAttribute.TryGetName(out var name).Should().BeTrue();
        name.Should().Be("x");

        fromAttribute.TryGetLength(out var length).Should().BeTrue();
        length.Should().Be(4);

        fromAttribute.TryGetTypeFromAttribute(out var typeFromAttribute).Should().BeTrue();
        typeFromAttribute.Should().Be(typeof(int));

        fromAttribute.TryGetTypeName(out var typeNameFromAttribute).Should().BeTrue();
        typeNameFromAttribute.Should().Be("int");

        var fromElementName = new XElement(XNamespace.Get(XmlOptions.Exs) + "int");

        fromElementName.TryGetTypeFromAttribute(out _).Should().BeFalse();
        fromElementName.TryGetTypeName(out var typeNameFromElement).Should().BeTrue();
        typeNameFromElement.Should().Be("int");

        fromElementName.TryGetElementType(out var elementType).Should().BeTrue();
        elementType.Should().Be(typeof(int));
    }
}

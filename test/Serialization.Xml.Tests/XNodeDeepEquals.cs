namespace vm2.Linq.Expressions.Serialization.Xml.Tests;

[ExcludeFromCodeCoverage]
public class XNodeDeepEquals(bool ignoreComments = false)
{
    Queue<string> _path = new(["."]);

    public string LastResult { get; private set; } = "";

    public bool False(string difference)
    {
        LastResult = $"FirstChild difference at {string.Join("/", _path)} in the {difference}.";
        _path.Clear();
        return false;
    }

    public bool AreEqual(XNode? left, XNode? right) => Equals(left, right);

    bool Equals(XNode? left, XNode? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null)
            return False($"left {right!.NodeType} is null");
        if (right is null)
            return False($"right {left.NodeType} is null");

        if (left.NodeType != right.NodeType)
            return False($"left is {left.NodeType} and right is {right.NodeType}");

        return left switch {
            XComment l => Equals(l, (XComment)right),
            XProcessingInstruction l => Equals(l, (XProcessingInstruction)right),
            XDocumentType l => Equals(l, (XDocumentType)right),
            XText l => Equals(l, (XText)right),
            XDocument l => Equals(l, (XDocument)right),
            XElement l => Equals(l, (XElement)right),
            _ => throw new InternalTransformErrorException("Unknown XNode type.")
        };
    }

    bool Equals(XComment left, XComment right)
    {
        if (left.BaseUri != right.BaseUri)
            return False($"comments base URI: \"{left.BaseUri}\" != \"{right.BaseUri}\"") || ignoreComments;
        if (left.Value != right.Value)
            return False($"comments: \"{left.Value}\" != \"{right.Value}\"") || ignoreComments;

        return true;
    }

    bool Equals(XProcessingInstruction left, XProcessingInstruction right)
    {
        if (left.BaseUri != right.BaseUri)
            return False($"processing instructions base URI: \"{left.BaseUri}\" != \"{right.BaseUri}\"");
        if (left.Target != right.Target)
            return False($"processing instructions target: \"{left.Target}\" != \"{right.Target}\"");
        if (left.Data != right.Data)
            return False($"processing instructions data: \"{left.Data}\" != \"{right.Data}\"");

        return true;
    }

    bool Equals(XDocumentType left, XDocumentType right)
    {
        if (left.BaseUri != right.BaseUri)
            return False($"DTD base URI: \"{left.BaseUri}\" != \"{right.BaseUri}\"");
        if (left.Name != right.Name)
            return False($"DTD name: \"{left.Name}\" != \"{right.Name}\"");
        if (left.PublicId != right.PublicId)
            return False($"public ID: \"{left.PublicId}\" != \"{right.PublicId}\"");
        if (left.SystemId != right.SystemId)
            return False($"system ID: \"{left.SystemId}\" != \"{right.SystemId}\"");
        if (left.InternalSubset != right.InternalSubset)
            return False($"internal subset: \"{left.InternalSubset}\" != \"{right.InternalSubset}\"");

        return true;
    }

    bool Equals(XText left, XText right)
    {
        if (left.BaseUri != right.BaseUri)
            return False($"text base URI: \"{left.BaseUri}\" != \"{right.BaseUri}\"");
        if (left.Value != right.Value)
            return False($"text _value: \"{left.Value}\" != \"{right.Value}\"");

        return true;
    }

    bool Equals(XDeclaration? left, XDeclaration? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null)
            return False("left declaration is null");
        if (right is null)
            return False("right declaration is null");

        if (left.Version != right.Version)
            return False($"document declaration version: \"{left.Version}\" != \"{right.Version}\"");
        if (left.Encoding != right.Encoding)
            return False($"document declaration encoding: \"{left.Encoding}\" != \"{right.Encoding}\"");
        if (left.Standalone != right.Standalone)
            return False($"document declaration standalone: \"{left.Standalone}\" != \"{right.Standalone}\"");

        return true;
    }

    bool Equals(XDocument left, XDocument right)
    {
        _path.Dequeue();

        if (left.BaseUri != right.BaseUri)
            return False($"document base URI: \"{left.BaseUri}\" != \"{right.BaseUri}\"");
        if (!Equals(left.Declaration, right.Declaration))
            return false;
        if (!Equals(left.DocumentType as XNode, right.DocumentType))
            return false;

        _path.Enqueue("/");

        return Equals(left.Root as XNode, right.Root);
    }

    bool Equals(XElement left, XElement right)
    {
        _path.Enqueue(left.Name.ToString());

        if (left.Name.ToString() != right.Name.ToString())
            return False($"element names: \"{left.Name}\" != \"{right.Name}\"");
        if (left.Attributes().Count() != right.Attributes().Count())
            return False($"element's number of attributes: \"{left.Attributes().Count()}\" != \"{right.Attributes().Count()}\"");
        if (left.Nodes().Count() != right.Nodes().Count())
            return False($"element's number of sub-nodes: \"{left.Nodes().Count()}\" != \"{right.Nodes().Count()}\"");

        if (left.HasAttributes)
        {
            var lEnum = left.Attributes().GetEnumerator();
            var rEnum = right.Attributes().GetEnumerator();

            while (lEnum.MoveNext() && rEnum.MoveNext())
            {
                var la = lEnum.Current;
                var ra = rEnum.Current;

                if (la!.Name.ToString() != ra!.Name.ToString())
                    return False($"attribute names: \"{la.Name}\" != \"{ra.Name}\"");
                if (la!.Value != ra!.Value)
                    return False($"attribute {la.Name} values: \"{la.Value}\" != \"{ra.Value}\"");
            }
        }

        if (left.Nodes().Any())
        {
            var lEnum = left.Nodes().GetEnumerator();
            var rEnum = right.Nodes().GetEnumerator();

            while (lEnum.MoveNext() && rEnum.MoveNext())
            {
                var la = lEnum.Current;
                var ra = rEnum.Current;

                if (!Equals(la, ra))
                    return false;
            }
        }

        _path.Dequeue();
        return true;
    }
}

namespace vm2.Linq.Expressions.Serialization.Xml.Tests;

public class XmlOptionsTests
{
    [Theory]
    [InlineData("ASCII",     "ascii")]
    [InlineData("ascii",     "ascii")]
    [InlineData("UTF-8",     "utf-8")]
    [InlineData("utf-8",     "utf-8")]
    [InlineData("UTF-16",    "utf-16")]
    [InlineData("utf-16",    "utf-16")]
    [InlineData("UTF-32",    "utf-32")]
    [InlineData("utf-32",    "utf-32")]
    [InlineData("ISO-8859-1","iso-8859-1")]
    [InlineData("LATIN1",    "iso-8859-1")]
    public void CharacterEncoding_NormalizesInput(string input, string expected)
    {
        var options = new XmlOptions { CharacterEncoding = input };

        options.CharacterEncoding.Should().Be(expected);
    }

    [Fact]
    public void CharacterEncoding_UnsupportedThrows()
    {
        var options = new XmlOptions();

        var act = () => options.CharacterEncoding = "EBCDIC";

        act.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void Encoding_Ascii()
    {
        var options = new XmlOptions { CharacterEncoding = "ASCII" };

        options.Encoding.Should().Be(Encoding.ASCII);
    }

    [Fact]
    public void Encoding_Utf8_Default()
    {
        var options = new XmlOptions();

        options.Encoding.Should().BeOfType<UTF8Encoding>();
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true,  false)]
    [InlineData(true,  true)]
    public void Encoding_Utf16(bool bigEndian, bool bom)
    {
        var options = new XmlOptions {
            CharacterEncoding = "UTF-16",
            BigEndian = bigEndian,
            ByteOrderMark = bom,
        };

        var encoding = options.Encoding;

        encoding.Should().BeOfType<UnicodeEncoding>();
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true,  true)]
    public void Encoding_Utf32(bool bigEndian, bool bom)
    {
        var options = new XmlOptions {
            CharacterEncoding = "UTF-32",
            BigEndian = bigEndian,
            ByteOrderMark = bom,
        };

        var encoding = options.Encoding;

        encoding.Should().BeOfType<UTF32Encoding>();
    }

    [Fact]
    public void Encoding_Latin1()
    {
        var options = new XmlOptions { CharacterEncoding = "ISO-8859-1" };

        options.Encoding.Should().Be(Encoding.Latin1);
    }
}

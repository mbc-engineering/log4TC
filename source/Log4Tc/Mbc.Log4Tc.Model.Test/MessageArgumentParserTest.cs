using FluentAssertions;
using System.Linq;
using Xunit;

namespace Mbc.Log4Tc.Model.Test
{
    public class MessageArgumentParserTest
    {
        [Fact]
        public void ParseArguments_WithEmptyString_ShouldReturnNoArguments()
        {
            // Arrange
            var parser = new MessageArgumentParser(string.Empty);

            // Act
            var args = parser.ParseArguments().ToList();

            // Assert
            args.Should().BeEmpty();
        }

        [Fact]
        public void ParseArguments_WithStringWithoutArgs_ShouldReturnNoArguments()
        {
            // Arrange
            var parser = new MessageArgumentParser("foobar");

            // Act
            var args = parser.ParseArguments().ToList();

            // Assert
            args.Should().BeEmpty();
        }

        [Fact]
        public void ParseArguments_WithSimpleArgs_ShouldReturnArguments()
        {
            // Arrange
            var parser = new MessageArgumentParser("foobar {0} {foo}");

            // Act
            var args = parser.ParseArguments().ToList();

            // Assert
            args.Should().BeEquivalentTo("0", "foo");
        }

        [Fact]
        public void ParseArguments_WithEscapedArgs_ShouldSkipThose()
        {
            // Arrange
            var parser = new MessageArgumentParser("foobar {{0}} {{{foo}}} {bar}");

            // Act
            var args = parser.ParseArguments().ToList();

            // Assert
            args.Should().BeEquivalentTo("foo", "bar");
        }

        [Fact]
        public void FormatMessage_WithNoArgs_ShouldReturnMessage()
        {
            // Arrange
            var testee = new MessageArgumentParser("Foo");

            // Act
            var formatted = testee.FormatMessage(Enumerable.Empty<object>());

            // Assert
            formatted.Should().Be("Foo");
        }

        [Fact]
        public void FormatMessage_WithNumericArgs_ShouldReturnFormattedMessage()
        {
            // Arrange
            var testee = new MessageArgumentParser("Foo {0} {1}");

            // Act
            var formatted = testee.FormatMessage(new object[] { "baz", 42 });

            // Assert
            formatted.Should().Be("Foo baz 42");
        }

        [Fact]
        public void FormatMessage_WithNumericOutOfOrderArgs_ShouldReturnFormattedMessage()
        {
            // Arrange
            var testee = new MessageArgumentParser("Foo {1} {0}");

            // Act
            var formatted = testee.FormatMessage(new object[] { "baz", 42 });

            // Assert
            formatted.Should().Be("Foo 42 baz");
        }

        [Fact]
        public void FormatMessage_WithNamedArgs_ShouldReturnFormattedMessage()
        {
            // Arrange
            var testee = new MessageArgumentParser("Foo {aString} {aInt}");

            // Act
            var formatted = testee.FormatMessage(new object[] { "baz", 42 });

            // Assert
            formatted.Should().Be("Foo baz 42");
        }

        [Fact]
        public void FormatMessage_WithEscapedBraces_ShouldReturnLiteral()
        {
            // Arrange
            var testee = new MessageArgumentParser("Foo {{{0}}} {{1}}");

            // Act
            var formatted = testee.FormatMessage(new object[] { "baz" });

            // Assert
            formatted.Should().Be("Foo {baz} {1}");
        }

        [Fact]
        public void FormatMessage_WithMissingArg_ShouldReplaceQuestionMark()
        {
            // Arrange
            var testee = new MessageArgumentParser("Foo {0}");

            // Act
            var formatted = testee.FormatMessage(new object[0]);

            // Assert
            formatted.Should().Be("Foo ?");
        }

        [Fact]
        public void FormatMessage_WithAlignment_ShouldAlignArguments()
        {
            // Arrange
            var testee = new MessageArgumentParser("({0,5})-({1,-5})");

            // Act
            var formatted = testee.FormatMessage(new object[] { "baz", 42 });

            // Assert
            formatted.Should().Be("(  baz)-(42   )");
        }

        [Fact]
        public void FormatMessage_WithFormat_ShouldFormatArguments()
        {
            // Arrange
            var testee = new MessageArgumentParser("({0:D4})-({1:E2})-({2:F2})-({3:N2})-({4:P1})-({5:X4})");

            // Act
            var formatted = testee.FormatMessage(new object[] { 42, 42, 42, 4200, 0.42, 42 });

            // Assert
            formatted.Should().Be("(0042)-(4.20E+001)-(42.00)-(4’200.00)-(42.0%)-(002A)");
        }


        [Fact]
        public void FormatMessage_WithAllFeatures_ShouldFormatArguments()
        {
            // Arrange
            var testee = new MessageArgumentParser("Named {foo} Index {2} Aligned {1,4} Formatted {foo:D4} Aligned+Formatted {5,6:D4}");

            // Act
            var formatted = testee.FormatMessage(new object[] { "baz", 42, "foo", 42, 0, 42 });

            // Assert
            formatted.Should().Be("Named baz Index foo Aligned   42 Formatted 0042 Aligned+Formatted   0042");
        }
    }
}

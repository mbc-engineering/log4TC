using FluentAssertions;
using FluentAssertions.Common;
using Mbc.Log4Tc.Model.Message;
using Optional;
using Optional.Collections;
using Optional.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Mbc.Log4Tc.Model.Test.Message
{
    public class MessageFormatterTest
    {
        public MessageFormatterTest()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [Fact]
        public void Arguments_WithEmptyString_ShouldReturnNoArguments()
        {
            // Arrange
            var parser = new MessageFormatter(string.Empty);

            // Act
            var args = parser.Arguments.ToList();

            // Assert
            args.Should().BeEmpty();
        }

        [Fact]
        public void Arguments_WithStringWithoutArgs_ShouldReturnNoArguments()
        {
            // Arrange
            var parser = new MessageFormatter("foobar");

            // Act
            var args = parser.Arguments.ToList();

            // Assert
            args.Should().BeEmpty();
        }

        [Fact]
        public void Arguments_WithSimpleArgs_ShouldReturnArguments()
        {
            // Arrange
            var parser = new MessageFormatter("foobar {0} {foo}");

            // Act
            var args = parser.Arguments.ToList();

            // Assert
            args.Should().BeEquivalentTo("0", "foo");
        }

        [Fact]
        public void Arguments_WithEscapedArgs_ShouldSkipThose()
        {
            // Arrange
            var parser = new MessageFormatter("foobar {{0}} {{{foo}}} {bar}");

            // Act
            var args = parser.Arguments.ToList();

            // Assert
            args.Should().BeEquivalentTo("foo", "bar");
        }

        [Fact]
        public void PositionalArguments_WithPositionalArgs_ShouldReturnSome()
        {
            // Arrange
            var parser = new MessageFormatter("{0} {1} {5}");

            // Act
            var positional = parser.PositionalArguments;

            // Assert
            positional.ValueOr(new int[0]).Should().BeEquivalentTo(new[] { 0, 1, 5 });
        }

        [Fact]
        public void PositionalArguments_WithNamedArgs_ShouldReturnNone()
        {
            // Arrange
            var parser = new MessageFormatter("{0} {1} {foo}");

            // Act
            var positional = parser.PositionalArguments;

            // Assert
            positional.Should().Be(Option.None<IEnumerable<int>>());
        }

        [Fact]
        public void Format_WithNoArgs_ShouldReturnMessage()
        {
            // Arrange
            var testee = new MessageFormatter("Foo");

            // Act
            var formatted = testee.Format(Enumerable.Empty<object>());

            // Assert
            formatted.Should().Be("Foo");
        }

        [Fact]
        public void Format_WithNumericArgs_ShouldReturnFormattedMessage()
        {
            // Arrange
            var testee = new MessageFormatter("Foo {0} {1}");

            // Act
            var formatted = testee.Format(new object[] { "baz", 42 });

            // Assert
            formatted.Should().Be("Foo baz 42");
        }

        [Fact]
        public void Format_WithNumericOutOfOrderArgs_ShouldReturnFormattedMessage()
        {
            // Arrange
            var testee = new MessageFormatter("Foo {1} {0}");

            // Act
            var formatted = testee.Format(new object[] { "baz", 42 });

            // Assert
            formatted.Should().Be("Foo 42 baz");
        }

        [Fact]
        public void Format_WithNamedArgs_ShouldReturnFormattedMessage()
        {
            // Arrange
            var testee = new MessageFormatter("Foo {aString} {aInt}");

            // Act
            var formatted = testee.Format(new object[] { "baz", 42 });

            // Assert
            formatted.Should().Be("Foo baz 42");
        }

        [Fact]
        public void Format_WithEscapedBraces_ShouldReturnLiteral()
        {
            // Arrange
            var testee = new MessageFormatter("Foo {{{0}}} {{1}}");

            // Act
            var formatted = testee.Format(new object[] { "baz" });

            // Assert
            formatted.Should().Be("Foo {baz} {1}");
        }

        [Fact]
        public void Format_WithMissingArg_ShouldReplaceQuestionMark()
        {
            // Arrange
            var testee = new MessageFormatter("Foo {0}");

            // Act
            var formatted = testee.Format(new object[0]);

            // Assert
            formatted.Should().Be("Foo ?");
        }

        [Fact]
        public void Format_WithAlignment_ShouldAlignArguments()
        {
            // Arrange
            var testee = new MessageFormatter("({0,5})-({1,-5})");

            // Act
            var formatted = testee.Format(new object[] { "baz", 42 });

            // Assert
            formatted.Should().Be("(  baz)-(42   )");
        }

        [Fact]
        public void Format_WithFormat_ShouldFormatArguments()
        {
            // Arrange
            var testee = new MessageFormatter("({0:D4})-({1:E2})-({2:F2})-({3:N2})-({4:P1})-({5:X4})");

            // Act
            var formatted = testee.Format(new object[] { 42, 42, 42, 4200, 0.42, 42 });

            // Assert
            formatted.Should().Be("(0042)-(4.20E+001)-(42.00)-(4,200.00)-(42.0 %)-(002A)");
        }

        [Fact]
        public void Format_WithAllFeatures_ShouldFormatArguments()
        {
            // Arrange
            var testee = new MessageFormatter("Named {foo} Aligned {baz,4} Formatted {bar:D4} Aligned+Formatted {foobar,6:D4}");

            // Act
            var formatted = testee.Format(new object[] { "baz", 42, 42, 42 });

            // Assert
            formatted.Should().Be("Named baz Aligned   42 Formatted 0042 Aligned+Formatted   0042");
        }

    }
}

using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Mbc.Log4Tc.Model.Test
{
    public class LogEntryTest
    {

        [Fact]
        public void FormattedMessage_WithNoArgs_ShouldReturnMessage()
        {
            // Arrange
            var logEntry = new LogEntry { Message = "Foo" };

            // Act
            var formattedMessage = logEntry.FormattedMessage;

            // Assert
            formattedMessage.Should().Be("Foo");
        }

        [Fact]
        public void FormattedMessage_WithNumericArgs_ShouldReturnFormattedMessage()
        {
            // Arrange
            var logEntry = new LogEntry
            {
                Message = "Foo {0} {1}",
            };

            logEntry.Arguments.Add(1, "baz");
            logEntry.Arguments.Add(2, 42);

            // Act
            var formattedMessage = logEntry.FormattedMessage;

            // Assert
            formattedMessage.Should().Be("Foo baz 42");
        }

        [Fact]
        public void FormattedMessage_WithNamedArgs_ShouldReturnFormattedMessage()
        {
            // Arrange
            var logEntry = new LogEntry
            {
                Message = "Foo {aString} {aInt}",
            };

            logEntry.Arguments.Add(1, "baz");
            logEntry.Arguments.Add(2, 42);

            // Act
            var formattedMessage = logEntry.FormattedMessage;

            // Assert
            formattedMessage.Should().Be("Foo baz 42");
        }

        [Fact]
        public void FormattedMessage_WithEscapedBraces_ShouldReturnLiteral()
        {
            // Arrange
            var logEntry = new LogEntry { Message = "Foo {{{0}}} {{1}}" };
            logEntry.Arguments.Add(1, "baz");

            // Act
            var formattedMessage = logEntry.FormattedMessage;

            // Assert
            formattedMessage.Should().Be("Foo {baz} {1}");
        }

        [Fact]
        public void FormattedMessage_WithNumeric_ShouldReturnPositionalArguments()
        {
            // Arrange
            var logEntry = new LogEntry { Message = "{0} {2} {1} {4} {0}" };
            logEntry.Arguments.Add(1, 1);
            logEntry.Arguments.Add(2, 2);
            logEntry.Arguments.Add(3, 3);
            logEntry.Arguments.Add(5, 5);

            // Act
            var formattedMessage = logEntry.FormattedMessage;

            // Assert
            formattedMessage.Should().Be("1 3 2 5 1");
        }

        [Fact]
        public void FormattedMessage_WithMixed_ShouldReturnNamedArguments()
        {
            // Arrange
            var logEntry = new LogEntry { Message = "{args} {2} {1} {4} {foo}" };
            logEntry.Arguments.Add(1, 1);
            logEntry.Arguments.Add(2, 2);
            logEntry.Arguments.Add(3, 3);
            logEntry.Arguments.Add(4, 4);
            logEntry.Arguments.Add(5, 5);

            // Act
            var formattedMessage = logEntry.FormattedMessage;

            // Assert
            formattedMessage.Should().Be("1 2 3 4 5");
        }

        [Fact]
        public void ArgumentValues_WithoutHoles_ShouldReturnValues()
        {
            // Arrange
            var logEntry = new LogEntry();
            logEntry.Arguments.Add(1, 10);
            logEntry.Arguments.Add(2, 20);
            logEntry.Arguments.Add(3, 30);

            // Act
            var values = logEntry.ArgumentValues;

            // Assert
            values.Should().BeEquivalentTo([10, 20, 30]);
        }

        [Fact]
        public void ArgumentValues_WithHoles_ShouldReturnValues()
        {
            // Arrange
            var logEntry = new LogEntry();
            logEntry.Arguments.Add(1, 10);
            logEntry.Arguments.Add(3, 30);

            // Act
            var values = logEntry.ArgumentValues;

            // Assert
            values.Should().BeEquivalentTo(new List<object>() { 10, null, 30 });
        }
    }
}

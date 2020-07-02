using FluentAssertions;
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
    }
}

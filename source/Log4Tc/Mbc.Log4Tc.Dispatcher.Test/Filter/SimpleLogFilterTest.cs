using FluentAssertions;
using Mbc.Log4Tc.Dispatcher.Filter;
using Mbc.Log4Tc.Model;
using Optional;
using Xunit;

namespace Mbc.Log4TcDispatcher.Test.Filter
{
    public class SimpleLogFilterTest
    {
        [Fact]
        public void Matches_WithLevel_ShouldMatchLevel()
        {
            // Arrange
            var filter = new SimpleLogFilter(level: Option.Some(LogLevel.Warn));

            // Act + Assert
            filter.Level.Should().Be(Option.Some(LogLevel.Warn));
            filter.Logger.Should().Be(Option.None<string>());
            filter.Matches(new LogEntry { Level = LogLevel.Warn }).Should().BeTrue();
            filter.Matches(new LogEntry { Level = LogLevel.Error }).Should().BeTrue();
            filter.Matches(new LogEntry { Level = LogLevel.Info }).Should().BeFalse();
        }

        [Fact]
        public void Matches_WithLogger_ShouldMatchFullString()
        {
            // Arrange
            var filter = new SimpleLogFilter(logger: Option.Some("foo.bar"));

            // Act + Assert
            filter.Level.Should().Be(Option.None<LogLevel>());
            filter.Logger.Should().Be(Option.Some("foo.bar"));
            filter.Matches(new LogEntry { Logger = "foo.bar" }).Should().BeTrue();
            filter.Matches(new LogEntry { Logger = "foo.bar.baz" }).Should().BeFalse();
            filter.Matches(new LogEntry { Logger = "baz.foo.bar" }).Should().BeFalse();
        }

        [Fact]
        public void Matches_WithLogger_ShouldMatchPrefixString()
        {
            // Arrange
            var filter = new SimpleLogFilter(logger: Option.Some("foo.bar.*"));

            // Act + Assert
            filter.Level.Should().Be(Option.None<LogLevel>());
            filter.Logger.Should().Be(Option.Some("foo.bar.*"));
            filter.Matches(new LogEntry { Logger = "foo.bar" }).Should().BeFalse();
            filter.Matches(new LogEntry { Logger = "foo.bar.baz" }).Should().BeTrue();
            filter.Matches(new LogEntry { Logger = "baz.foo.bar" }).Should().BeFalse();
        }

        [Fact]
        public void Matches_WithLogger_ShouldMatchSuffixString()
        {
            // Arrange
            var filter = new SimpleLogFilter(logger: Option.Some("*.foo.bar"));

            // Act + Assert
            filter.Level.Should().Be(Option.None<LogLevel>());
            filter.Logger.Should().Be(Option.Some("*.foo.bar"));
            filter.Matches(new LogEntry { Logger = "foo.bar" }).Should().BeFalse();
            filter.Matches(new LogEntry { Logger = "foo.bar.baz" }).Should().BeFalse();
            filter.Matches(new LogEntry { Logger = "baz.foo.bar" }).Should().BeTrue();
        }

        [Fact]
        public void Matches_WithLogger_ShouldMatchPrefixAndSuffixString()
        {
            // Arrange
            var filter = new SimpleLogFilter(logger: Option.Some("*foo.bar*"));

            // Act + Assert
            filter.Level.Should().Be(Option.None<LogLevel>());
            filter.Logger.Should().Be(Option.Some("*foo.bar*"));
            filter.Matches(new LogEntry { Logger = "foo" }).Should().BeFalse();
            filter.Matches(new LogEntry { Logger = "foo.bar" }).Should().BeTrue();
            filter.Matches(new LogEntry { Logger = "foo.bar.baz" }).Should().BeTrue();
            filter.Matches(new LogEntry { Logger = "baz.foo.bar" }).Should().BeTrue();
        }

        [Fact]
        public void Matches_WithMultipleCriteria_ShouldMatchAll()
        {
            // Arrange
            var filter = new SimpleLogFilter(level: Option.Some(LogLevel.Warn), logger: Option.Some("foo.bar"));

            // Act + Assert
            filter.Level.Should().Be(Option.Some(LogLevel.Warn));
            filter.Logger.Should().Be(Option.Some("foo.bar"));
            filter.Matches(new LogEntry { Level = LogLevel.Warn, Logger = "foo.bar" }).Should().BeTrue();
            filter.Matches(new LogEntry { Level = LogLevel.Info, Logger = "foo.bar" }).Should().BeFalse();
            filter.Matches(new LogEntry { Level = LogLevel.Warn, Logger = "foo.bar.baz" }).Should().BeFalse();
        }

        [Fact]
        public void ToString_ShouldReturnCriteria()
        {
            new SimpleLogFilter(level: Option.Some(LogLevel.Warn), logger: Option.Some("foo.bar")).ToString()
                .Should().Be("Filter(Level >= Warn && Logger == foo.bar)");
            new SimpleLogFilter(logger: Option.Some("foo.bar")).ToString()
                .Should().Be("Filter(Logger == foo.bar)");
            new SimpleLogFilter(level: Option.Some(LogLevel.Warn)).ToString()
                .Should().Be("Filter(Level >= Warn)");
        }
    }
}

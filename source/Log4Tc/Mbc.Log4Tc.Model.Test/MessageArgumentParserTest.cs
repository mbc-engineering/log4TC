using FluentAssertions;
using System;
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
    }
}

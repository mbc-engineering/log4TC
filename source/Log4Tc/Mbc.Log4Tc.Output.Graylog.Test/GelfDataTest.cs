using FluentAssertions;
using Xunit;

namespace Mbc.Log4Tc.Output.Graylog.Test
{
    public class GelfDataTest
    {
        [Fact]
        public void SerializeGelfData_ShouldMatchSpec()
        {
            // Arrange
            var gelf = new GelfData
            {
                Host = "example.org",
                ShortMessage = "A short message that helps you identify what is going on",
                FullMessage = "Backtrace here more stuff",
                Timestamp = 1385053862.3072M,
                Level = 1,
            };

            gelf.Add("user_id", 9001);
            gelf.Add("_some_info", "foo");

            // Act
            var json = gelf.ToJson();

            // Assert
            json.Should().Be("{\"version\":\"1.1\",\"host\":\"example.org\",\"short_message\":\"A short message that helps you identify what is going on\",\"full_message\":\"Backtrace here more stuff\",\"timestamp\":1385053862.3072,\"level\":1,\"_user_id\":9001,\"_some_info\":\"foo\"}");
        }
    }
}

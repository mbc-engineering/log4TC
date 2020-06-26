using FluentAssertions;
using Mbc.Log4Tc.Dispatcher.Filter;
using Microsoft.Extensions.Configuration;
using Optional;
using System.Linq;
using Xunit;

namespace Mbc.Log4TcDispatcher.Test.Filter
{
    public class FilterConfigurationFactoryTest
    {
        [Fact]
        public void Create_EmptyFilter_ShouldReturnAllFilter()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().AddJsonStream(GetType().Assembly.GetManifestResourceStream("Mbc.Log4TcDispatcher.Test.Filter.assets.EmptyFilter.json")).Build();

            // Act
            var filter = FilterConfigurationFactory.Create(configuration.GetSection("Filter"), AllMatchFilter.Default);

            // Assert
            filter.Should().BeSameAs(AllMatchFilter.Default);
        }

        [Fact]
        public void Create_WithStringFilter_ShouldReturnAllFilter()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().AddJsonStream(GetType().Assembly.GetManifestResourceStream("Mbc.Log4TcDispatcher.Test.Filter.assets.StringFilter.json")).Build();

            // Act
            var filter = FilterConfigurationFactory.Create(configuration.GetSection("Filter"), AllMatchFilter.Default);

            // Assert
            filter.Should().BeSameAs(AllMatchFilter.Default);
        }

        [Fact]
        public void Create_WithSingleSimpleFilter_ShouldReturnSimpleFilter()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().AddJsonStream(GetType().Assembly.GetManifestResourceStream("Mbc.Log4TcDispatcher.Test.Filter.assets.SingleSimpleFilter.json")).Build();

            // Act
            var filter = FilterConfigurationFactory.Create(configuration.GetSection("Filter"), null);

            // Assert
            filter.Should().BeOfType<SimpleLogFilter>();
            ((SimpleLogFilter)filter).Should().BeEquivalentTo(new SimpleLogFilter(logger: Option.Some("foo")));
        }

        [Fact]
        public void Create_WithMultipleSimpleFilter_ShouldReturnOrWithSimpleFilter()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().AddJsonStream(GetType().Assembly.GetManifestResourceStream("Mbc.Log4TcDispatcher.Test.Filter.assets.MultipleSimpleFilter.json")).Build();

            // Act
            var filter = FilterConfigurationFactory.Create(configuration.GetSection("Filter"), null);

            // Assert
            filter.Should().BeOfType<OrFilter>().Which.Filter.Should().HaveCount(2).And.AllBeOfType<SimpleLogFilter>();
            ((SimpleLogFilter)((OrFilter)filter).Filter.ElementAt(0)).Should().BeEquivalentTo(new SimpleLogFilter(logger: Option.Some("foo")));
            ((SimpleLogFilter)((OrFilter)filter).Filter.ElementAt(1)).Should().BeEquivalentTo(new SimpleLogFilter(logger: Option.Some("bar")));
        }
    }
}

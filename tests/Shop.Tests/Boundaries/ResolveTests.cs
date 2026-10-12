using Shop.Data;

namespace Shop.Tests.Boundaries
{
    /// <summary>
    /// Keeps the harness boundary real when tests run: a test gets application services, and nothing behind them.
    /// </summary>
    public class ResolveTests : TestBase
    {
        [Fact]
        public void Resolve_ShouldFailTheTest_When_TheTestAsksForTheDbContext()
        {
            // Arrange:
            // A test asks for the database context, to write rows past the factory

            // Act
            var act = () => Resolve<ShopDbContext>();

            // Assert
            act.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void Resolve_ShouldFailTheTest_When_TheTestAsksForTheServiceProvider()
        {
            // Arrange:
            // A test asks for the container, to resolve anything it likes

            // Act
            var act = () => Resolve<IServiceProvider>();

            // Assert
            act.ShouldThrow<InvalidOperationException>();
        }
    }
}
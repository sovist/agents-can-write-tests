using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shop.Api;
using Shop.Tests.EntityFactory;

namespace Shop.Tests.Base
{
    /// <summary>
    /// Boots the application's own composition root over a private in-memory SQLite database, and hands out application services only.
    /// </summary>
    /// <remarks>
    /// Every <see cref="Resolve{T}"/> opens a new scope, as a new request would. So a read goes to the database, not to a context that still holds the entity.
    /// </remarks>
    public abstract class TestBase : IDisposable
    {
        private readonly SqliteConnection _connection;

        private readonly ServiceProvider _serviceProvider;

        private readonly List<IServiceScope> _scopes = new List<IServiceScope>();

        /// <summary>
        /// The default and preferred way to create test data.
        /// </summary>
        protected ShopFactory ShopFactory { get; }

        protected TestBase()
        {
            // The in-memory database lives as long as this connection stays open: one per test.
            _connection = new SqliteConnection("DataSource=:memory:");

            _connection.Open();

            _serviceProvider = new ServiceCollection()
                .AddShop(_ => _.UseSqlite(_connection))
                .BuildServiceProvider(new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                });

            _serviceProvider.EnsureShopDatabase();

            ShopFactory = new ShopFactory(_serviceProvider);
        }

        /// <summary>
        /// An application service, from a new scope. The constraint is the boundary: a test cannot ask
        /// for the service provider, a database context, or anything else that is not an entry point.
        /// </summary>
        protected T Resolve<T>()
            where T : IApplicationService
        {
            var scope = _serviceProvider.CreateScope();

            _scopes.Add(scope);

            return scope.ServiceProvider.GetRequiredService<T>();
        }

        public void Dispose()
        {
            foreach (var scope in _scopes)
            {
                scope.Dispose();
            }

            _serviceProvider.Dispose();

            _connection.Dispose();
        }
    }
}
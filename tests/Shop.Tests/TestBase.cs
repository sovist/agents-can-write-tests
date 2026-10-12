using Shop.Api;

namespace Shop.Tests
{
    /// <summary>
    /// The base for this project's tests. It runs on the shared harness from <c>Shop.Tests.Base</c>,
    /// and lets tests resolve application services only.
    /// Helpers that only this project's tests need go here.
    /// </summary>
    public abstract class TestBase : Shop.Tests.Base.TestBase
    {
        /// <summary>
        /// An application service, from a new scope. Anything else fails the test: a test cannot ask for
        /// the service provider, a database context, or anything else that is not an entry point.
        /// </summary>
        protected override T Resolve<T>()
        {
            if (!typeof(IApplicationService).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException($"Tests resolve application services only, not {typeof(T).Name}. Create data with ShopFactory, and read results back through an application service.");
            }

            return base.Resolve<T>();
        }
    }
}
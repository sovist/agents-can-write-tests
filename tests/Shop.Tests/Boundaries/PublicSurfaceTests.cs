using System.Reflection;
using System.Runtime.CompilerServices;
using Shop.Api;
using Shop.Api.Dto;
using Shop.Data;

namespace Shop.Tests.Boundaries
{
    /// <summary>
    /// Keeps the harness boundary real: a test reaches only what a host can, and data only through the factory.
    /// The Api project holds the entry points and, in its Dto folder, what they exchange.
    /// The Data project holds entities and the database context. Only the factory gets a database context instance.
    /// The service project exposes nothing but its composition root.
    /// </summary>
    public class PublicSurfaceTests
    {
        private static readonly Assembly ApiAssembly = typeof(IApplicationService).Assembly;

        private static readonly Assembly DataAssembly = typeof(ShopDbContext).Assembly;

        private static readonly Assembly ServiceAssembly = typeof(ShopComposition).Assembly;

        [Fact]
        public void ServiceAssembly_ShouldExposeOnlyTheCompositionRoot()
        {
            // Arrange:
            // Application services and managers are internal to the service project

            // Act
            var exported = ServiceAssembly
                .GetExportedTypes()
                .Select(_ => _.FullName);

            // Assert
            exported.ShouldBe(new[] { typeof(ShopComposition).FullName }, "A public type lets a test reach past the application services. Test it through them instead.");
        }

        [Fact]
        public void ServiceAssembly_ShouldNotOpenItsInternalsToTests()
        {
            // Act
            var grants = ServiceAssembly
                .GetCustomAttributes<InternalsVisibleToAttribute>()
                .Select(_ => _.AssemblyName);

            // Assert
            grants.ShouldBeEmpty("InternalsVisibleTo lets a test reach past the application services. Test through them instead.");
        }

        [Fact]
        public void ApiAssembly_ShouldExposeOnlyApplicationServicesAndDtos()
        {
            // Arrange:
            // A DTO means data: a class with properties only, an enum, or the exception the services throw

            // Act
            var unexpected = ApiAssembly
                .GetExportedTypes()
                .Where(_ => !IsApplicationService(_) && !IsDto(_))
                .Select(_ => _.FullName);

            // Assert
            unexpected.ShouldBeEmpty("The Api project holds application service interfaces and the DTOs they exchange. Put behavior behind an application service.");
        }

        [Fact]
        public void DataAssembly_ShouldExposeOnlyEntitiesAndTheDbContext()
        {
            // Arrange:
            // An entity means data: a class with properties only. Rules stay in the managers of the service project

            // Act
            var unexpected = DataAssembly
                .GetExportedTypes()
                .Where(_ => _ != typeof(ShopDbContext) && !HasOnlyProperties(_))
                .Select(_ => _.FullName);

            // Assert
            unexpected.ShouldBeEmpty("The Data project holds entities and the database context. Put rules in a manager of the service project.");
        }

        private static bool IsApplicationService(Type type)
        {
            return type.IsInterface && typeof(IApplicationService).IsAssignableFrom(type);
        }

        private static bool IsDto(Type type)
        {
            if (type.Namespace != typeof(OrderDto).Namespace)
            {
                return false;
            }

            return type.IsEnum || typeof(Exception).IsAssignableFrom(type) || HasOnlyProperties(type);
        }

        private static bool HasOnlyProperties(Type type)
        {
            return type.IsClass
                && type
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                    .All(_ => _.IsSpecialName);
        }
    }
}
using Shop.Api.Dto;

namespace Shop.Tests.Extensions
{
    public static class ShouldlyExtensions
    {
        /// <summary>
        /// Asserts that the task throws a <see cref="ShopException"/> of the given type.
        /// </summary>
        public static async Task ShouldThrowAsync(this Task task, ShopExceptionType type)
        {
            var exception = await task.ShouldThrowAsync<ShopException>();

            exception.Type.ShouldBe(type);
        }
    }
}
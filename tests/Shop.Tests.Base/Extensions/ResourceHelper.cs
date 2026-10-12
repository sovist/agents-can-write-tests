using System.IO;
using System.Text;

namespace Shop.Tests.Extensions
{
    public static class ResourceHelper
    {
        /// <summary>
        /// Reads, as UTF-8 text, the first embedded resource of the type's assembly whose name contains
        /// <paramref name="fileName"/>. The folder path can be left out of <paramref name="fileName"/>.
        /// </summary>
        public static string GetResourceFileContent(this Type type, string fileName)
        {
            var assembly = type.Assembly;

            var fullFileName = assembly.GetManifestResourceNames().FirstOrDefault(_ => _.Contains(fileName));

            if (string.IsNullOrEmpty(fullFileName))
            {
                throw new InvalidOperationException($"Resource file [{fileName}] not found in assembly [{assembly.FullName}].");
            }

            using (var reader = new StreamReader(assembly.GetManifestResourceStream(fullFileName), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
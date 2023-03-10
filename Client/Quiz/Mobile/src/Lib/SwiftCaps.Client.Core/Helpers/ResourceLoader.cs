using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SwiftCaps.Client.Core.Helpers
{
    public static class ResourceLoader
    {
        /// <summary>
        /// Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource stream.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        public static Stream GetEmbeddedResourceStream(Assembly assembly, string resourceFileName)
        {
            var resourceName = FormatResourceName(assembly, resourceFileName);
            return assembly.GetManifestResourceStream(resourceName);
        }

        /// <summary>
        /// Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource as a byte array.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        public static byte[] GetEmbeddedResourceBytes(Assembly assembly, string resourceFileName)
        {
            var stream = GetEmbeddedResourceStream(assembly, resourceFileName);

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Attempts to find and return the given resource from within the specified assembly.
        /// </summary>
        /// <returns>The embedded resource as a string.</returns>
        /// <param name="assembly">Assembly.</param>
        /// <param name="resourceFileName">Resource file name.</param>
        public static string GetEmbeddedResourceString(Assembly assembly, string resourceFileName)
        {
            var stream = GetEmbeddedResourceStream(assembly, resourceFileName);

            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets the string embedded resource from within the specified assembly type.
        /// </summary>
        /// <param name="type">The type of object or classes within the assembly.</param>
        /// <param name="resourceName">Name of the resource.</param>
        public static string GetEmbeddedResourceString(Type type, string resourceName)
        {
            return GetEmbeddedResourceString(type.GetTypeInfo().Assembly, resourceName);
        }

        /// <summary>
        /// Formats the name of the resource.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="resourceName">Name of the resource.</param>
        private static string FormatResourceName(Assembly assembly, string resourceName)
        {
            resourceName = assembly.GetName()
                .Name + "." + resourceName.Replace(" ", "_")
                .Replace("\\", ".").Replace("/", ".");

            var resourceNames = assembly.GetManifestResourceNames();

            var resourcePaths = resourceNames
                .Where(x => x.EndsWith(resourceName, StringComparison.CurrentCultureIgnoreCase))
                .ToArray();

            if (!resourcePaths.Any())
                throw new Exception($"Resource ending with {resourceName} not found.");

            if (resourcePaths.Length > 1)
                throw new Exception(
                    $"Multiple resources ending with {resourceName} found: \n{string.Join(Environment.NewLine, resourcePaths)}");

            return resourcePaths.Single();

        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Peritos.Common.Reflection
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Loads and returns all assemblies in the current application base directory,
        /// excluding system and Microsoft assemblies and common runtime libraries.
        /// </summary>
        /// <returns>An array of loaded <see cref="Assembly"/> instances representing the solution's assemblies.</returns>
        public static Assembly[] GetSolutionAssemblies()
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                                .Where(x => !x.Contains("Microsoft.") && !x.Contains("System.") && !x.Contains("vcruntime140") && !x.Contains("msvcp140."))
                                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)));
            return assemblies.ToArray();
        }
    }
}

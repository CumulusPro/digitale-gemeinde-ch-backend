using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Peritos.Common.Reflection
{
    public static class AssemblyExtensions
    {
        public static Assembly[] GetSolutionAssemblies()
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                                .Where(x => !x.Contains("Microsoft.") && !x.Contains("System.") && !x.Contains("vcruntime140") && !x.Contains("msvcp140."))
                                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)));
            return assemblies.ToArray();
        }
    }
}

using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace Peritos.Common.Mapping
{
    public static class MappingExtensions
    {
        /// <summary>
        /// Creates mappings for all types in AssemblyA to AssemblyB that share a name, but only types that match a TypeFilter
        /// Common use cases include Data.UserDto => Services.UserDto if the properties are identical. 
        /// </summary>
        /// <param name="profile">The Automapper Profile object</param>
        /// <param name="assemblyFrom">Assembly to map From</param>
        /// <param name="assemblyTo">Assembly to map To</param>
        /// <param name="typeFilter">A filter to only map these types e.g. x.Name.EndsWith("Parameters")</param>
        /// <param name="mapReverse">True to also map the same types from AssemblyTo => AssemblyFrom </param>
        /// <param name=trimmedSuffixed">If you need to map MyEntityDto to MyEntityModel, then you can pass in the suffix of Dto and Model to have them trimmed and ignored when matching.</param>
        public static void CreateMappingOnName(this Profile profile, 
                                          Assembly assemblyFrom, 
                                          Assembly assemblyTo, 
                                          Func<Type, bool> typeFilter, 
                                          bool mapReverse = false,
                                          string[] trimmedSuffixes = null)
        {
            var typesFrom = assemblyFrom.GetTypes();
            var typesTo = assemblyTo.GetTypes();

            foreach (var typeFrom in typesFrom.Where(typeFilter))
            {
                //Find types in assembly B with the same name. 
                Type typeTo =  typesTo.FirstOrDefault(x => TrimEnd(x.Name, trimmedSuffixes).Equals(TrimEnd(typeFrom.Name, trimmedSuffixes), StringComparison.InvariantCultureIgnoreCase));

                if (typeTo != null)
                {
                    //Map only one way  from service to data project. 
                    profile.CreateMap(typeFrom, typeTo);
                    if(mapReverse)
                    {
                        profile.CreateMap(typeTo, typeFrom);
                    }
                }
            }
        }

        private static string TrimEnd(string source, string[] values)
        {
            if (values == null || values.Length == 0)
                return source;

            foreach(var value in values)
            {
                if (source.EndsWith(value))
                {
                    return TrimEnd(source.Remove(source.LastIndexOf(value)), values);
                }
            }

            return source;
        }
    }
}

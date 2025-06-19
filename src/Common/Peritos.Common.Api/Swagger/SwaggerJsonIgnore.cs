using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace Peritos.Common.Api.Swagger
{
    /// <summary>
    /// An <see cref="IOperationFilter"/> implementation for removing parameters from Swagger documentation
    /// that are marked with <see cref="JsonIgnoreAttribute"/>.
    /// </summary>
    public class SwaggerJsonIgnore : IOperationFilter
    {
        /// <summary>
        /// Applies the filter to the Swagger operation. This method removes any parameters from the Swagger UI
        /// that correspond to properties marked with <see cref="JsonIgnoreAttribute"/> in the action's parameter types.
        /// </summary>
        /// <param name="operation">The Swagger operation to modify.</param>
        /// <param name="context">The context that contains metadata about the operation.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var ignoredProperties = context.MethodInfo.GetParameters()
                .SelectMany(p => p.ParameterType.GetProperties()
                                 .Where(prop => prop.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                                 );

            if (ignoredProperties.Any())
            {
                foreach (var property in ignoredProperties)
                {
                    operation.Parameters = operation.Parameters
                        .Where(p => !p.Name.Equals(property.Name, StringComparison.InvariantCulture))
                        .ToList();
                }

            }
        }
    }
}

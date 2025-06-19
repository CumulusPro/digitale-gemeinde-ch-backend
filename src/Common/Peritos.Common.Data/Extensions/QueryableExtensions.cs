using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Peritos.Common.Data.Extensions
{
    /// <summary>
    /// Provides extension methods for IQueryable to support dynamic sorting and pagination.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Asynchronously creates a paged list from an IQueryable source based on the specified paging parameters.
        /// Supports dynamic sorting by multiple fields separated by commas.
        /// </summary>
        /// <typeparam name="T">The type of the IQueryable elements.</typeparam>
        /// <param name="dbSet">The IQueryable source to paginate.</param>
        /// <param name="pagingParameters">The paging parameters including page number, page size, and ordering details.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a list of paged items.</returns>
        public static async Task<List<T>> ToPagedList<T>(this IQueryable<T> dbSet, PagingParameters pagingParameters) where T : class
        {
            if (!string.IsNullOrWhiteSpace(pagingParameters.OrderBy))
            {
                foreach (var field in pagingParameters.OrderBy.Split(','))
                {
                    dbSet = dbSet.OrderBy(field, pagingParameters.OrderByDirection == PagingParametersOrderByDirectionEnum.Ascending);
                }
            }

            return await dbSet
                        .Skip((pagingParameters.Page - 1) * pagingParameters.PageSize)
                        .Take(pagingParameters.PageSize)
                        .ToListAsync();
        }

        /// <summary>
        /// Dynamically applies ordering to an IQueryable source based on a property name and sort direction.
        /// This method handles both initial ordering and subsequent ordering (ThenBy).
        /// </summary>
        /// <typeparam name="T">The type of the IQueryable elements.</typeparam>
        /// <param name="source">The IQueryable source to order.</param>
        /// <param name="property">The property name to sort by. Can be a nested property using dot notation (e.g., "Address.City").</param>
        /// <param name="ascending">If true, applies ascending order; otherwise, descending order.</param>
        /// <returns>An ordered IQueryable with the applied sorting.</returns>
        public static IOrderedQueryable<T> OrderBy<T>(
            this IQueryable<T> source,
            string property,
            bool ascending)
        {
            //If it's already ordered, then you need to use ThenBy. 
            if (source.Expression.Type == typeof(IOrderedQueryable<T>))
            {
                return ApplyOrder<T>(source, property, ascending ? "ThenBy" : "ThenByDescending");
            }
            else
            {
                return ApplyOrder<T>(source, property, ascending ? "OrderBy" : "OrderByDescending");
            }
        }

        /// <summary>
        /// Uses reflection and expression trees to apply a dynamic OrderBy or ThenBy clause on the IQueryable source.
        /// </summary>
        /// <typeparam name="T">The type of the IQueryable elements.</typeparam>
        /// <param name="source">The IQueryable source to order.</param>
        /// <param name="property">The property name to sort by. Supports nested properties separated by '.'</param>
        /// <param name="methodName">The LINQ method name to apply ("OrderBy", "OrderByDescending", "ThenBy", or "ThenByDescending").</param>
        /// <returns>An ordered IQueryable with the specified sorting applied.</returns>
        static IOrderedQueryable<T> ApplyOrder<T>(
            IQueryable<T> source,
            string property,
            string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }
    }
}

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
    public static class QueryableExtensions
    {
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

using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Data.BaseModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Peritos.Common.Data
{
    /// <summary>
    /// Base repository implementation for data access, providing common CRUD operations.
    /// Supports querying with filtering, including navigation properties, and audit-aware deletion.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TContext">The database context type, derived from <see cref="BaseContext{TContext}"/>.</typeparam>
    public class RepositoryBase<T, TContext> : IRepository<T> where T : class
                                                                        where TContext : BaseContext<TContext>
    {
        protected readonly DbSet<T> _dbSet;
        protected readonly TContext _context;
        private readonly IRequestContext _requestContext;

        protected RepositoryBase(TContext context, IRequestContext requestContext = null)
        {
            _context = context;
            _requestContext = requestContext;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Get an entity using delegate
        /// </summary>
        /// <param name="where">Query filters</param>
        /// <returns>Result set</returns>
        public IQueryable<T> GetAll(Expression<Func<T, bool>> where)
        {
            //Simple call GetAllWithInclude with no includes. 
            return GetAllWithInclude(where);
        }

        /// <summary>
        /// Get an entity using delegate
        /// </summary>
        /// <param name="where">Query filters</param>
        /// <returns>Result set</returns>
        public async Task<T> Get(Expression<Func<T, bool>> where)
        {
            //GetAll as IQueryable then FirstOrDefaultAsync. 
            return await GetAll(where).AsNoTracking().FirstOrDefaultAsync<T>();
        }

        /// <summary>
        /// Gets entities using delegate
        /// </summary>
        /// <param name="where">Query filters</param>
        /// <param name="includeProperties">Properties to select</param>
        /// <returns>Result set</returns>
        public IQueryable<T> GetAllWithInclude(
                Expression<Func<T, bool>> where,
                params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> result = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                result = result.Include(includeProperty);
            }

            return result.Where(where);
        }

        /// <summary>
        /// Retrieves a single entity matching the specified predicate, including related navigation properties asynchronously.
        /// </summary>
        /// <param name="where">A LINQ expression to filter entities.</param>
        /// <param name="includeProperties">Navigation properties to include.</param>
        /// <returns>The first matching entity or null if none found.</returns>
        public async Task<T> GetWithInclude(
        Expression<Func<T, bool>> where,
        params Expression<Func<T, object>>[] includeProperties)
        {
            return await GetAllWithInclude(where, includeProperties).FirstOrDefaultAsync<T>();
        }

        /// <summary>
        /// Commit any changes to the database
        /// </summary>
        /// <returns>Empty task</returns>
        public virtual async Task Commit()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                Console.WriteLine();
                throw;
            }
        }

        /// <summary>
        /// Deletes the specified entity. For entities inheriting from <see cref="Auditable"/>,
        /// performs a soft delete by setting the deletion date and user; otherwise, deletes physically.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="commitChanges">If true, commits the deletion immediately.</param>
        /// <returns>The deleted entity.</returns>
        public async Task<T> Delete(T entity, bool commitChanges = true)
        {
            //If the type we are trying to delete is auditable, then we don't actually delete it but instead set it to be updated with a delete date. 
            if (typeof(Auditable).IsAssignableFrom(typeof(T)))
            {
                (entity as Auditable).DateDeleted = DateTimeOffset.UtcNow;
                (entity as Auditable).DeletedBy = _requestContext != null ? _requestContext.UserId : null;
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                _dbSet.Remove(entity);
            }

            if (commitChanges)
            {
                await Commit();
            }

            return entity;
        }

        /// <summary>
        /// Inserts the specified entity into the database.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <param name="commitChanges">If true, commits the insertion immediately.</param>
        /// <returns>The inserted entity.</returns>
        public async Task<T> Insert(T entity, bool commitChanges = true)
        {
            _dbSet.Add(entity);
            if (commitChanges)
            {
                await Commit();
            }
            return entity;
        }

        /// <summary>
        /// Updates the specified entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="partitionKey">Optional partition key identifier, default is "id".</param>
        /// <param name="commitChanges">If true, commits the update immediately.</param>
        /// <returns>The updated entity.</returns>
        public async Task<T> Update(T entity, string partitionKey = "id", bool commitChanges = true)
        {
            _dbSet.Update(entity);
            if (commitChanges)
            {
                await Commit();
            }

            return entity;
        }
    }
}

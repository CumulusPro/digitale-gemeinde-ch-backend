using System.Threading.Tasks;

namespace Peritos.Common.Data
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="commitChanges">Commit changes immediately</param>
        Task<T> Delete(T entity, bool commitChanges = true);

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="commitChanges">Commit changes immediately</param>
        Task<T> Insert(T entity, bool commitChanges = true);

        /// <summary>
        /// Update an item. 
        /// </summary>
        /// <param name="entity">Item to update</param>
        /// <param name="commitChanges">Commit changes immediately</param>
        /// <returns></returns>
        Task<T> Update(T entity, string partitionKey="id", bool commitChanges = true);

        /// <summary>
        /// Commit any changes to the database
        /// </summary>
        /// <returns>Empty task</returns>
        Task Commit();
    }
}

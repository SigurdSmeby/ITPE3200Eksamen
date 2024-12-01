using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sub_Application_1.Repositories.Interfaces
{
    /// <summary>
    /// Generic repository interface for basic CRUD operations.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface IRepository<T> where T : class
    {
        // Get all entities
        Task<IEnumerable<T>> GetAllAsync();

        // Get entity by ID
        Task<T?> GetByIdAsync(int id);

        // Add a new entity
        Task AddAsync(T entity);

        // Update an existing entity
        void Update(T entity);

        // Delete an entity
        void Delete(T entity);

        // Save changes to the database
        Task SaveAsync();
    }
}

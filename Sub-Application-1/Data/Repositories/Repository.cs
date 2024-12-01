using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sub_Application_1.Data;
using Sub_Application_1.Repositories.Interfaces;

namespace Sub_Application_1.Repositories
{
    /// <summary>
    /// Generic repository implementation for basic CRUD operations.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <inheritdoc />
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <inheritdoc />
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <inheritdoc />
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <inheritdoc />
        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <inheritdoc />
        public virtual async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

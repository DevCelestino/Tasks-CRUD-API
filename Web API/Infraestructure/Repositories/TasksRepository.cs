using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infraestructure.Data;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Implementation of the ITasksRepository interface for managing task entities in the database.
    /// Initializes a new instance of the <see cref="TasksRepository"/> class.
    /// </summary>
    /// <param name="context">The database context to use for data access.</param>
    public class TasksRepository(AppDbContext context) : ITasksRepository
    {
        #region Properties

        /// <summary>
        /// The database context used for interacting with the database.
        /// </summary>
        private readonly AppDbContext _context = context;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async Task<List<TasksEntity>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Tasks
                .Where(task => ids.Contains(task.Id))
                .Include(task => task.Person)
                .ToListAsync() ?? [];
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TasksEntity>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(task => task.Person)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<TasksEntity> AddAsync(TasksEntity task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            
            return task;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(TasksEntity task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(TasksEntity task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task Detach(TasksEntity task)
        {
            var entry = _context.Entry(task);
            if (entry.State == EntityState.Detached)
            {
                return;
            }

            entry.State = EntityState.Detached;

            await Task.CompletedTask;
        }

        #endregion
    }
}

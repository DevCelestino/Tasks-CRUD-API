using Domain.Entities;
using Infrastructure.Data;
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
        public async Task<TasksEntity> AddAsync(TasksEntity task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return task;
        }

        #endregion
    }
}

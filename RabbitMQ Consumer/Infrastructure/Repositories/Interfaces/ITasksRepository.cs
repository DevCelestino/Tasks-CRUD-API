using Domain.Entities;

namespace Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Interface for managing tasks in the repository.
    /// </summary>
    public interface ITasksRepository
    {
        /// <summary>
        /// Adds a new task asynchronously.
        /// </summary>
        /// <param name="task">The task entity to be added.</param>
        /// <returns>The added task entity.</returns>
        Task<TasksEntity> AddAsync(TasksEntity task);
    }
}

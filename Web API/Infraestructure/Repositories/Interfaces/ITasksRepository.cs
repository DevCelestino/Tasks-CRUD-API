using Domain.Entities;

namespace Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Interface for performing operations related to tasks in the database.
    /// </summary>
    public interface ITasksRepository
    {
        /// <summary>
        /// Retrieves a list of tasks based on their unique identifiers.
        /// </summary>
        /// <param name="ids">A list of unique identifiers for the tasks to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing a list of retrieved tasks.</returns>
        Task<List<TasksEntity>> GetByIdsAsync(List<int> ids);

        /// <summary>
        /// Retrieves all tasks from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing an enumerable of all tasks.</returns>
        Task<IEnumerable<TasksEntity>> GetAllAsync();

        /// <summary>
        /// Adds a new task to the database.
        /// </summary>
        /// <param name="task">The task entity to be added.</param>
        /// <returns>A task representing the asynchronous operation, containing the added task entity.</returns>
        Task<TasksEntity> AddAsync(TasksEntity task);

        /// <summary>
        /// Updates an existing task in the database.
        /// </summary>
        /// <param name="task">The task entity with updated information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateAsync(TasksEntity task);

        /// <summary>
        /// Deletes a task from the database.
        /// </summary>
        /// <param name="task">The task entity to be deleted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(TasksEntity task);

        /// <summary>
        /// Detaches the specified task entity from the DbContext, 
        /// preventing it from being tracked for changes. 
        /// </summary>
        /// <param name="task">The task entity to detach.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Detach(TasksEntity task);
    }
}

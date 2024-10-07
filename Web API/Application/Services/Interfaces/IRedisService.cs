using Domain.Entities;

namespace Application.Services.Interfaces
{
    /// <summary>
    /// Interface for interacting with Redis to cache and retrieve entities.
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// Retrieves a list of task entities from Redis using their IDs.
        /// </summary>
        /// <param name="ids">A list of task IDs to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation, containing a list of <see cref="TasksEntity"/>.</returns>
        Task<List<TasksEntity>> GetTaskByIdsAsync(List<int> ids);

        /// <summary>
        /// Retrieves a list of person entities from Redis using their IDs.
        /// </summary>
        /// <param name="ids">A list of person IDs to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation, containing a list of <see cref="PersonsEntity"/>.</returns>
        Task<List<PersonsEntity>> GetPersonsByIdsAsync(List<int> ids);

        /// <summary>
        /// Removes a cached value from Redis using the specified cache key.
        /// </summary>
        /// <param name="cacheKey">The key of the cached value to remove.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RemoveCachedValue(string cacheKey);
    }
}

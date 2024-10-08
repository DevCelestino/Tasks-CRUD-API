using Domain.Entities;

namespace Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Interface for performing operations related to persons in the database.
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Retrieves a collection of persons based on their unique identifiers.
        /// </summary>
        /// <param name="ids">A list of unique identifiers for the persons to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of retrieved persons.</returns>
        Task<IEnumerable<PersonsEntity>> GetByIdsAsync(List<int> ids);

        /// <summary>
        /// Retrieves all persons from the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing an enumerable of all persons.</returns>
        Task<IEnumerable<PersonsEntity>> GetAllAsync();

        /// <summary>
        /// Detaches the specified <see cref="TasksEntity"/> from the tracking context.
        /// </summary>
        /// <param name="task">The task entity to detach from the context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Detach(PersonsEntity person);
    }
}

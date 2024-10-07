using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infraestructure.Data;
using Infrastructure.Repositories.Interfaces;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing operations related to persons in the database.
    /// Initializes a new instance of the <see cref="PersonsRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for accessing person data.</param>
    public class PersonsRepository(AppDbContext context) : IPersonsRepository
    {
        #region Properties

        /// <summary>
        /// The database context used for interacting with the database.
        /// </summary>
        private readonly AppDbContext _context = context;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async Task<IEnumerable<PersonsEntity>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Persons
                .Where(person => ids.Contains(person.Id))
                .Include(person => person.Tasks)
                .ToListAsync() ?? [];
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PersonsEntity>> GetAllAsync()
        {
            return await _context.Persons
                .Include(person => person.Tasks)
                .ToListAsync();
        }

        #endregion
    }
}

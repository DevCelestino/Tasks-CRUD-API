using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data
{
    /// <summary>
    /// Represents the database context for the application, managing entity sets for Persons and Tasks.
    /// Initializes a new instance of the <see cref="AppDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        #region DbSets

        /// <summary>
        /// Gets or sets the DbSet for <see cref="PersonsEntity"/>.
        /// </summary>
        public DbSet<PersonsEntity> Persons { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for <see cref="TasksEntity"/>.
        /// </summary>
        public DbSet<TasksEntity> Tasks { get; set; }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TasksEntity>()
                .HasOne(t => t.Person)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.PersonId);
        }

        #endregion
    }
}

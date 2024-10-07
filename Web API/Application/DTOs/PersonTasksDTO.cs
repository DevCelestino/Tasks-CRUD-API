namespace Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for representing a person and their associated tasks.
    /// </summary>
    public class PersonTasksDTO
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for each person.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the person.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of tasks associated with the person.
        /// </summary>
        public ICollection<TasksDTO>? Tasks { get; set; }

        #endregion
    }
}

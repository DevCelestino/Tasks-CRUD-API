using Domain.Enumerables;

namespace Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for representing a task.
    /// </summary>
    public class TasksDTO
    {
        #region Properties

        /// <summary>
        /// An unique id of each task.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The identifier of the person associated with the task.
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// The person associated with the task, represented as a data transfer object.
        /// </summary>
        /// <remarks>
        /// This property is optional and may be null if not loaded.
        /// </remarks>
        public PersonsDTO? Person { get; set; }

        /// <summary>
        /// A string that specifies the task title.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// A string that specifies the task description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// A string that specifies the task location.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// A TaskSeverity enumerable that specifies the task severity.
        /// </summary>
        public TaskSeverityEnum Severity { get; set; }

        /// <summary>
        /// A DateTime that specifies the task start date and time.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// A DateTime that specifies the task end date and time.
        /// </summary>
        public DateTime? EndDate { get; set; }

        #endregion
    }
}

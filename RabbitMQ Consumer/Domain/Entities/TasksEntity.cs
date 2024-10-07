using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Enumerables;

namespace Domain.Entities
{
    /// <summary>
    /// Represents a task in the system, containing details such as title, description, 
    /// location, severity, and time frames for start and end dates. Each instance corresponds 
    /// to a record in the "tasks" table.
    /// </summary>
    [Table("tasks")]
    public class TasksEntity
    {
        #region Properties

        /// <summary>
        /// An unique id of each task.
        /// </summary>
        [Column("id"), Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// The identifier of the person associated with the task.
        /// </summary>
        [Column("personid"), Required]
        public int PersonId { get; set; }

        /// <summary>
        /// The person associated with the task.
        /// </summary>
        [ForeignKey("PersonId")]
        public virtual PersonsEntity? Person { get; set; }

        /// <summary>
        /// A string that specifies the task title.
        /// </summary>
        [Column("title"), Required, StringLength(30)]
        public required string Title { get; set; }

        /// <summary>
        /// A string that specifies the task description.
        /// </summary>
        [Column("description"), StringLength(100)]
        public string? Description { get; set; }

        /// <summary>
        /// A string that specifies the task location.
        /// </summary>
        [Column("location"), StringLength(100)]
        public string? Location { get; set; }

        /// <summary>
        /// A TaskSeverity enumerable that specifies the task severity.
        /// </summary>
        [Column("severity"), Required]
        public TaskSeverityEnum Severity { get; set; }

        /// <summary>
        /// A DateTime that specifies the task start date and time.
        /// </summary>
        [Column("startdate"), Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// A DateTime that specifies the task end date and time.
        /// </summary>
        [Column("enddate")]
        public DateTime? EndDate { get; set; }

        #endregion
    }
}

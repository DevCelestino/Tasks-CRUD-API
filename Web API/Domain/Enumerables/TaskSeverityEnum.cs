namespace Domain.Enumerables
{
    /// <summary>
    /// Represents the severity levels of tasks in the system.
    /// The severity can be categorized as Low, Medium, High, or Critical.
    /// </summary>
    public enum TaskSeverityEnum
    {
        /// <summary>
        /// Indicates a low severity task.
        /// </summary>
        Low = 1,

        /// <summary>
        /// Indicates a medium severity task.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Indicates a high severity task.
        /// </summary>
        High = 3,

        /// <summary>
        /// Indicates a critical severity task.
        /// </summary>
        Critical = 4
    }
}

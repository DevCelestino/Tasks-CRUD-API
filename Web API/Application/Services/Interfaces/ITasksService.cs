using Application.DTOs;

namespace Application.Services.Interfaces
{
    /// <summary>
    /// Interface for task management services.
    /// </summary>
    public interface ITasksService
    {
        /// <summary>
        /// Retrieves a list of tasks by their IDs.
        /// </summary>
        /// <param name="ids">A list of task IDs to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, with a list of <see cref="TasksDTO"/> as a result.</returns>
        Task<List<TasksDTO>> GetById(List<int> ids);

        /// <summary>
        /// Retrieves tasks associated with specified person IDs.
        /// </summary>
        /// <param name="ids">A list of person IDs whose tasks are to be retrieved.</param>
        /// <returns>A task representing the asynchronous operation, with a list of <see cref="PersonTasksDTO"/> as a result.</returns>
        Task<List<PersonTasksDTO>> GetByPersonId(List<int> ids);

        /// <summary>
        /// Inserts a new task into the system by sending the task data to a message queue for processing.
        /// </summary>
        /// <param name="tasksDto">The task data transfer object containing the details of the task to be added.</param>
        /// <returns>A task representing the asynchronous operation, which does not return any value.</returns>
        Task Insert(TasksDTO tasksDto);

        /// <summary>
        /// Edits an existing task based on the provided <see cref="TasksDTO"/>.
        /// </summary>
        /// <param name="tasksDto">The task data transfer object containing updated task information.</param>
        /// <returns>A task representing the asynchronous operation, with the updated <see cref="TasksDTO"/> as a result.</returns>
        Task<TasksDTO> Edit(TasksDTO tasksDto);

        /// <summary>
        /// Deletes a task by its ID.
        /// </summary>
        /// <param name="id">The ID of the task to delete.</param>
        /// <returns>A task representing the asynchronous operation, with the deleted <see cref="TasksDTO"/> as a result.</returns>
        Task<TasksDTO> Delete(int id);
    }
}

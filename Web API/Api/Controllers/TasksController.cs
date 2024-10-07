using Microsoft.AspNetCore.Mvc;
using Api.Versioning;
using Application.DTOs;
using Application.Services.Interfaces;
using System.Text.Json;

namespace Api.Controllers
{
    /// <summary>
    /// Controller for managing tasks, providing endpoints for CRUD operations on tasks.
    /// </summary>
    /// <param name="logger">Logger for tracking operations and errors.</param>
    /// <param name="tasksService">Service for task-related operations.</param>
    [ApiController]
    [V1]
    [Route($"{VersioningConstant.Placeholder}/[controller]")]
    public class TasksController(
        ILogger<TasksController> logger,
        ITasksService tasksService) : ControllerBase
    {
        #region Properties

        /// <summary>
        /// Logger for the TasksController, used for logging information and errors.
        /// </summary>
        private readonly ILogger<TasksController> _logger = logger;

        /// <summary>
        /// Service for managing tasks, providing methods for task operations.
        /// </summary>
        private readonly ITasksService _tasksService = tasksService;

        #endregion

        #region Routes

        /// <summary>
        /// Retrieves tasks by their unique identifiers.
        /// </summary>
        /// <param name="id">List of task IDs to retrieve.</param>
        /// <returns>A list of tasks matching the provided IDs.</returns>
        [HttpGet]
        [V1]
        [ProducesResponseType(typeof(TasksDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTasksByIds([FromQuery] List<int> id)
        {
            try
            {
                var tasks = await _tasksService.GetById(id);

                _logger.LogInformation($"GET Tasks request successfully retrieved {tasks.Count} tasks.");
                return Ok(tasks);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"GET Tasks request failed with argument error: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET Tasks request failed with error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Retrieves tasks associated with the specified person IDs.
        /// </summary>
        /// <param name="id">List of person IDs to retrieve tasks for.</param>
        /// <returns>A list of persons with their associated tasks.</returns>
        [HttpGet("ByPersonIds")]
        [V1]
        [ProducesResponseType(typeof(TasksDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTasksByPersonIds([FromQuery] List<int> id)
        {
            try
            {
                var persons = await _tasksService.GetByPersonId(id);

                _logger.LogInformation($"GET Tasks/ByPersonIds request successfully retrieved {persons.Count} persons and {persons.Sum(person => person.Tasks?.Count)} tasks.");
                return Ok(persons);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"GET Tasks/ByPersonIds request failed with argument error: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET Tasks/ByPersonIds request failed with error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Adds a new task by accepting a <see cref="TasksDTO"/> from the request body.
        /// The task data is sent to a message queue for processing.
        /// </summary>
        /// <param name="taskDto">The task data transfer object containing the details of the task to be added.</param>
        /// <returns>A 200 OK response with a message indicating the task has been sent to RabbitMQ, or an error response if the request fails.</returns>
        [HttpPost]
        [V1]
        [ProducesResponseType(typeof(TasksDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddTask([FromBody] TasksDTO taskDto)
        {
            try
            {
                await _tasksService.Insert(taskDto);

                _logger.LogInformation($"Successfully sent task to RabbitMQ!");
                return Ok("Task sent to RabbitMQ!");
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError($"POST Tasks request failed due to null argument: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"POST Tasks request failed with argument error: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST Tasks request failed with error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Updates an existing task.
        /// </summary>
        /// <param name="taskDto">The updated task data transfer object.</param>
        /// <returns>The updated task.</returns>
        [HttpPut]
        [V1]
        [ProducesResponseType(typeof(TasksDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTask([FromBody] TasksDTO taskDto)
        {
            try
            {
                var task = await _tasksService.Edit(taskDto);

                _logger.LogInformation($"Successfully updated task: {JsonSerializer.Serialize(task, new JsonSerializerOptions { WriteIndented = true })}");
                return Ok(task);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError($"PUT Tasks request failed due to null argument: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"PUT Tasks request failed with argument error: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PUT Tasks request failed with error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Deletes a task by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the task to delete.</param>
        /// <returns>A confirmation message.</returns>
        [HttpDelete("{id}")]
        [V1]
        [ProducesResponseType(typeof(TasksDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _tasksService.Delete(id);

                _logger.LogInformation($"Successfully deleted task: {JsonSerializer.Serialize(task, new JsonSerializerOptions { WriteIndented = true })}");
                return Ok("Task successfully deleted!");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"DELETE Tasks request failed with argument error: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE Tasks request failed with error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        #endregion
    }
}

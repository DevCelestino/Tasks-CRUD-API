using Application.Services;
using Domain.Entities;
using Domain.Enumerables;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Repositories.Interfaces;
using Moq;

namespace Tests.Unit.Services
{
    /// <summary>
    /// Unit tests for the <see cref="TasksService"/> class, specifically testing the Delete functionality.
    /// </summary>
    [TestFixture]
    [Category("TasksService")]
    public class TasksServiceDeleteTests
    {
        #region Properties

        /// <summary>
        /// The instance of <see cref="TasksService"/> being tested.
        /// </summary>
        private TasksService _tasksService;

        /// <summary>
        /// Mock repository for tasks, simulating <see cref="ITasksRepository"/> interactions.
        /// </summary>
        private Mock<ITasksRepository> _tasksRepositoryMock;

        /// <summary>
        /// Mock repository for persons, simulating <see cref="IPersonsRepository"/> interactions.
        /// </summary>
        private Mock<IPersonsRepository> _personsRepositoryMock;

        /// <summary>
        /// Mock message sender, simulating <see cref="IMessageSender"/> interactions.
        /// </summary>
        private Mock<IMessageSender> _messageSenderMock;

        #endregion

        #region Setup

        /// <summary>
        /// Initializes the test class, setting up mocks and the service before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _tasksRepositoryMock = new Mock<ITasksRepository>();
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _messageSenderMock = new Mock<IMessageSender>();

            _tasksService = new TasksService(
                _tasksRepositoryMock.Object,
                _personsRepositoryMock.Object,
                _messageSenderMock.Object
            );
        }

        #endregion

        #region Tests

        /// <summary>
        /// Tests that when a task with the provided ID exists, the method returns the deleted task DTO.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("DeleteTask")]
        public async Task DeleteTask_ShouldReturnDeletedTaskDTO_WhenTaskWithProvidedIDExists()
        {
            var task = new TasksEntity
            {
                Id = 1,
                Title = "Test Task",
                PersonId = 1,
                Person = new PersonsEntity
                {
                    Id = 1,
                    Name = "Test Person"
                },
                Description = "Task Description",
                Location = "Online",
                Severity = TaskSeverityEnum.Low,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(1).AddHours(4),
            };

            _tasksRepositoryMock.Setup(repo => repo.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync([task]);

            var result = await _tasksService.Delete(task.Id);

            Assert.That(result.Id, Is.EqualTo(task.Id));
            Assert.That(result.Title, Is.EqualTo(task.Title));
            Assert.That(result.PersonId, Is.EqualTo(task.PersonId));
            Assert.That(result.Person?.Id, Is.EqualTo(task.Person.Id));
            Assert.That(result.Person?.Name, Is.EqualTo(task.Person.Name));
            Assert.That(result.Description, Is.EqualTo(task.Description));
            Assert.That(result.Location, Is.EqualTo(task.Location));
            Assert.That(result.Severity, Is.EqualTo(task.Severity));
            Assert.That(result.StartDate, Is.EqualTo(task.StartDate));
            Assert.That(result.EndDate, Is.EqualTo(task.EndDate));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that when trying to delete a task with a non-existing ID, an exception is thrown.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("ReadTask")]
        public async Task GetById_ShouldThrowException_WhenTaskWithProvidedIdDoesNotExist()
        {
            var id = 1;

            _tasksRepositoryMock.Setup(repo => repo.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync([]);

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Delete(id));
            Assert.That(ex.Message, Is.EqualTo($"No task found with ID {id}. (Parameter 'id')"));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that when an invalid ID is provided for deletion, an exception is thrown.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("ReadTask")]
        public async Task GetById_ShouldThrowException_WhenInvalidIdProvided()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Delete(-1));
            Assert.That(ex.Message, Is.EqualTo("Task ID must be greater than zero. (Parameter 'id')"));

            await Task.CompletedTask;
        }

        #endregion
    }
}

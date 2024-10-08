using Application.DTOs;
using Application.Services;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enumerables;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Repositories.Interfaces;
using Moq;
using StackExchange.Redis;

namespace Tests.Unit.Services
{
    /// <summary>
    /// Unit tests for the <see cref="TasksService"/> class, specifically testing the Create functionality.
    /// </summary>
    [TestFixture]
    [Category("TasksService")]
    public class TasksServiceCreateTests
    {
        #region Properties

        /// <summary>
        /// Mock for the Redis connection multiplexer, simulating interactions with Redis.
        /// </summary>
        private Mock<IConnectionMultiplexer> _redisMock;

        /// <summary>
        /// Instance of <see cref="RedisService"/> for interacting with Redis and repository data.
        /// </summary>
        private Mock<IRedisService> _redisServiceMock;

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
            // Mock Redis connection
            _redisMock = new Mock<IConnectionMultiplexer>();

            // Mock Redis service
            _redisServiceMock = new Mock<IRedisService>();

            // Mock repository for tasks and persons
            _tasksRepositoryMock = new Mock<ITasksRepository>();
            _personsRepositoryMock = new Mock<IPersonsRepository>();

            // Mock message sender
            _messageSenderMock = new Mock<IMessageSender>();

            // Initialize the TasksService with the mocked dependencies
            _tasksService = new TasksService(
                _redisServiceMock.Object,
                _tasksRepositoryMock.Object,
                _personsRepositoryMock.Object,
                _messageSenderMock.Object
            );
        }

        #endregion

        #region Tests

        /// <summary>
        /// Tests that a valid task DTO sends a message successfully.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        [Test]
        [Category("CreateTask")]
        public async Task CreateTask_ShouldSendMessage_WhenValidDataProvided()
        {
            var taskDto = new TasksDTO
            {
                Title = "New Task",
                PersonId = 1,
                Description = "Task Description",
                Location = "Online",
                Severity = TaskSeverityEnum.Low,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(1).AddHours(4),
            };

            _redisServiceMock.Setup(repo => repo.GetPersonsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<PersonsEntity>
                {
                    new PersonsEntity {
                        Id = 1,
                        Name = "Test Person"
                    }
                });

            await _tasksService.Insert(taskDto);

            _messageSenderMock.Verify(m => m.SendMessage(It.Is<TasksDTO>(t => t.Title == "New Task")), Times.Once);
        }

        /// <summary>
        /// Tests that an ArgumentNullException is thrown when a null task DTO is provided.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        [Test]
        [Category("CreateTask")]
        public async Task CreateTask_ShouldThrowException_WhenInvalidDTOProvided()
        {
            TasksDTO? taskDto = null;

#pragma warning disable CS8604 // Possible null reference argument.
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _tasksService.Insert(taskDto));
#pragma warning restore CS8604 // Possible null reference argument.
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null. (Parameter 'tasksDto')"));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that an ArgumentNullException is thrown when a null task DTO is provided.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        [Test]
        [Category("CreateTask")]
        public async Task CreateTask_ShouldThrowException_WhenInvalidPersonIDProvided()
        {
            var taskDto = new TasksDTO
            {
                Title = "",
                Description = "Task Description"
            };

            _redisServiceMock.Setup(repo => repo.GetPersonsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync([]);

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Insert(taskDto));
            Assert.That(ex.Message, Is.EqualTo("No person found with ID 0. (Parameter 'PersonId')"));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that an ArgumentException is thrown when an invalid title is provided in the task DTO.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        [Test]
        [Category("CreateTask")]
        public async Task CreateTask_ShouldThrowException_WhenInvalidTitleProvided()
        {
            var taskDto = new TasksDTO
            {
                Title = "",
                PersonId = 1,
                Description = "Task Description"
            };

            _redisServiceMock.Setup(repo => repo.GetPersonsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<PersonsEntity>
                {
                    new PersonsEntity {
                        Id = 1,
                        Name = "Test Person"
                    }
                });

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Insert(taskDto));
            Assert.That(ex.Message, Is.EqualTo("Title cannot be null or empty. (Parameter 'Title')"));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that an ArgumentException is thrown when an invalid severity value is provided in the task DTO.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        [Test]
        [Category("CreateTask")]
        public async Task CreateTask_ShouldThrowException_WhenInvalidSeverityProvided()
        {
            var taskDto = new TasksDTO
            {
                Title = "New Task",
                PersonId = 1,
                Description = "Task Description",
                Severity = 0
            };

            _redisServiceMock.Setup(repo => repo.GetPersonsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<PersonsEntity>
                {
                    new PersonsEntity {
                        Id = 1,
                        Name = "Test Person"
                    }
                });


            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Insert(taskDto));
            Assert.That(ex.Message, Is.EqualTo("Severity '0' is not a valid value. (Parameter 'Severity')"));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that an ArgumentException is thrown when an invalid start date is provided in the task DTO.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        [Test]
        [Category("CreateTask")]
        public async Task CreateTask_ShouldThrowException_WhenInvalidStartDateProvided()
        {
            var taskDto = new TasksDTO
            {
                Title = "New Task",
                PersonId = 1,
                Description = "Task Description",
                Severity = TaskSeverityEnum.Low,
                StartDate = DateTime.MinValue
            };

            _redisServiceMock.Setup(repo => repo.GetPersonsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<PersonsEntity>
                {
                    new PersonsEntity {
                        Id = 1,
                        Name = "Test Person"
                    }
                });


            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Insert(taskDto));
            Assert.That(ex.Message, Is.EqualTo("Start date is not in a valid format."));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that an ArgumentException is thrown when an invalid end date is provided in the task DTO.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        [Test]
        [Category("CreateTask")]
        public async Task CreateTask_ShouldThrowException_WhenInvalidEndDateProvided()
        {
            var taskDto = new TasksDTO
            {
                Title = "New Task",
                PersonId = 1,
                Description = "Task Description",
                Severity = TaskSeverityEnum.Low,
                StartDate = DateTime.Now.AddDays(1)
            };

            _redisServiceMock.Setup(repo => repo.GetPersonsByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<PersonsEntity>
                {
                    new PersonsEntity {
                        Id = 1,
                        Name = "Test Person"
                    }
                });


            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Insert(taskDto));
            Assert.That(ex.Message, Is.EqualTo("End date is not in a valid format."));

            await Task.CompletedTask;
        }

        #endregion
    }
}

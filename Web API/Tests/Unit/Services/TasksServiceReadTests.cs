using Application.DTOs;
using Application.Services;
using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Repositories.Interfaces;
using Moq;
using StackExchange.Redis;

namespace Tests.Unit.Services
{
    /// <summary>
    /// Unit tests for the <see cref="TasksService"/> class, specifically testing the Read functionality.
    /// </summary>
    [TestFixture]
    [Category("TasksService")]
    public class TasksServiceReadTests
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
        /// Tests that when tasks with the provided IDs exist, the method returns the corresponding tasks.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("ReadTask")]
        public async Task GetById_ShouldReturnTask_WhenTaskWithProvidedIDsExists()
        {
            var taskId = new List<int> { 1, 3, 6 };
            var expectedTask1 = new TasksEntity { Id = 1, Title = "Test Task 1" };
            var expectedTask6 = new TasksEntity { Id = 6, Title = "Test Task 6" };

            _redisServiceMock.Setup(repo => repo.GetTaskByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync([expectedTask1, expectedTask6]);

            var result = await _tasksService.GetById(taskId);
            var dto = new List<TasksDTO>()
            {
                new TasksDTO { Id = expectedTask1.Id, Title = expectedTask1.Title },
                new TasksDTO { Id = expectedTask6.Id, Title = expectedTask6.Title }
            };

            Assert.That(result.Count(), Is.EqualTo(dto.Count));
            Assert.That(result.Any(x => x.Id == expectedTask1.Id), Is.True);
            Assert.That(result.Any(x => x.Title == expectedTask1.Title), Is.True);
            Assert.That(result.Any(x => x.Id == expectedTask6.Id), Is.True);
            Assert.That(result.Any(x => x.Title == expectedTask6.Title), Is.True);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that when no task IDs are provided, the method returns all tasks.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("ReadTask")]
        public async Task GetById_ShouldReturnAllTask_WhenNoTasksIdsProvided()
        {
            var expectedTask1 = new TasksEntity { Id = 1, Title = "Test Task 1" };
            var expectedTask3 = new TasksEntity { Id = 3, Title = "Test Task 3" };
            var expectedTask6 = new TasksEntity { Id = 6, Title = "Test Task 6" };

            _tasksRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync([expectedTask1, expectedTask3, expectedTask6]);

            var result = await _tasksService.GetById([]);
            var dto = new List<TasksDTO>()
            {
                new TasksDTO { Id = expectedTask1.Id, Title = expectedTask1.Title },
                new TasksDTO { Id = expectedTask3.Id, Title = expectedTask3.Title },
                new TasksDTO { Id = expectedTask6.Id, Title = expectedTask6.Title }
            };

            Assert.That(result.Count(), Is.EqualTo(dto.Count));
            Assert.That(result.Any(x => x.Id == expectedTask1.Id), Is.True);
            Assert.That(result.Any(x => x.Title == expectedTask1.Title), Is.True);
            Assert.That(result.Any(x => x.Id == expectedTask3.Id), Is.True);
            Assert.That(result.Any(x => x.Title == expectedTask3.Title), Is.True);
            Assert.That(result.Any(x => x.Id == expectedTask6.Id), Is.True);
            Assert.That(result.Any(x => x.Title == expectedTask6.Title), Is.True);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that when tasks do not exist, an empty list is returned.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("ReadTask")]
        public async Task GetById_ShouldReturnEmptyList_WhenTasksDoesNotExist()
        {
            var result = await _tasksService.GetById([]);

            Assert.That(result.Count(), Is.EqualTo(0));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that when an invalid task ID is provided, an exception is thrown.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("ReadTask")]
        public async Task GetById_ShouldThrowException_WhenInvalidIdProvided()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.GetById([-1]));
            Assert.That(ex.Message, Is.EqualTo("All task IDs must be greater than zero. (Parameter 'ids')"));

            await Task.CompletedTask;
        }

        #endregion
    }
}

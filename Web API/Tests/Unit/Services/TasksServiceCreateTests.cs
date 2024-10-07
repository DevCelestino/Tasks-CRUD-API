using Application.DTOs;
using Application.Services;
using Domain.Entities;
using Domain.Enumerables;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Repositories.Interfaces;
using Moq;

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
        /// 
        /// </summary>
        /// <returns></returns>
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

            _personsRepositoryMock.Setup(repo => repo.GetByIdsAsync(It.IsAny<List<int>>()))
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
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
        [Test]
        [Category("CreateTask")]
        public async Task CreateTask_ShouldThrowException_WhenInvalidPersonIDProvided()
        {
            var taskDto = new TasksDTO
            {
                Title = "",
                Description = "Task Description"
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Insert(taskDto));
            Assert.That(ex.Message, Is.EqualTo("No person found with ID 0. (Parameter 'PersonId')"));

            await Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

            _personsRepositoryMock.Setup(repo => repo.GetByIdsAsync(It.IsAny<List<int>>()))
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
        /// 
        /// </summary>
        /// <returns></returns>
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

            _personsRepositoryMock.Setup(repo => repo.GetByIdsAsync(It.IsAny<List<int>>()))
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
        /// 
        /// </summary>
        /// <returns></returns>
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

            _personsRepositoryMock.Setup(repo => repo.GetByIdsAsync(It.IsAny<List<int>>()))
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
        /// 
        /// </summary>
        /// <returns></returns>
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

            _personsRepositoryMock.Setup(repo => repo.GetByIdsAsync(It.IsAny<List<int>>()))
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

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
    /// Unit tests for the <see cref="TasksService"/> class, specifically testing the Update functionality.
    /// </summary>
    [TestFixture]
    [Category("TasksService")]
    public class TasksServiceUpdateTests
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
        /// Tests that when valid task data is provided, the task is updated correctly.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("UpdateTask")]
        public async Task UpdateTask_ShouldUpdateTask_WhenValidDataProvided()
        {
            var taskDto = new TasksDTO
            {
                Id = 1,
                Title = "Edited Task",
                PersonId = 3,
                Description = "Edited Task Description",
                Location = "Online",
                Severity = TaskSeverityEnum.Low,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(1).AddHours(4),
            };

            _tasksRepositoryMock.Setup(repo => repo.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<TasksEntity>
                { new TasksEntity
                {
                    Id = 1,
                    Title = "Not Edited Task",
                    PersonId = 1,
                    Description = "Not Edited Task Description",
                    Location = "Room 1",
                    Severity = TaskSeverityEnum.High,
                    StartDate = DateTime.Now.AddDays(2),
                    EndDate = DateTime.Now.AddDays(2).AddHours(6),
                }});

            _personsRepositoryMock.Setup(repo => repo.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<PersonsEntity>
                {
                                new PersonsEntity {
                                    Id = 3,
                                    Name = "Test Person"
                                }
                });

            await _tasksService.Edit(taskDto);

            _tasksRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<TasksEntity>(t =>
                t.Id == taskDto.Id &&
                t.Title == taskDto.Title &&
                t.PersonId == taskDto.PersonId &&
                t.Description == taskDto.Description &&
                t.Location == taskDto.Location &&
                t.Severity == taskDto.Severity &&
                t.StartDate == taskDto.StartDate &&
                t.EndDate == taskDto.EndDate)), Times.Once);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that when an invalid DTO is provided, an exception is thrown.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("UpdateTask")]
        public async Task UpdateTask_ShouldThrowException_WhenInvalidDTOProvided()
        {
            TasksDTO? taskDto = null;

#pragma warning disable CS8604 // Possible null reference argument.
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _tasksService.Edit(taskDto));
#pragma warning restore CS8604 // Possible null reference argument.
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null. (Parameter 'tasksDto')"));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that when an invalid person ID is provided, an exception is thrown.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("UpdateTask")]
        public async Task UpdateTask_ShouldThrowException_WhenInvalidPersonIDProvided()
        {
            var taskDto = new TasksDTO
            {
                Title = "",
                Description = "Task Description"
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Edit(taskDto));
            Assert.That(ex.Message, Is.EqualTo("No person found with ID 0. (Parameter 'PersonId')"));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that when an invalid title is provided, an exception is thrown.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("UpdateTask")]
        public async Task UpdateTask_ShouldThrowException_WhenInvalidTitleProvided()
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

            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Edit(taskDto));
            Assert.That(ex.Message, Is.EqualTo("Title cannot be null or empty. (Parameter 'Title')"));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that when an invalid severity is provided, an exception is thrown.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("UpdateTask")]
        public async Task UpdateTask_ShouldThrowException_WhenInvalidSeverityProvided()
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


            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Edit(taskDto));
            Assert.That(ex.Message, Is.EqualTo("Severity '0' is not a valid value. (Parameter 'Severity')"));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that when an invalid start date is provided, an exception is thrown.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("UpdateTask")]
        public async Task UpdateTask_ShouldThrowException_WhenInvalidStartDateProvided()
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


            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Edit(taskDto));
            Assert.That(ex.Message, Is.EqualTo("Start date is not in a valid format."));

            await Task.CompletedTask;
        }

        /// <summary>
        /// Tests that when an invalid end date is provided, an exception is thrown.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Test]
        [Category("UpdateTask")]
        public async Task UpdateTask_ShouldThrowException_WhenInvalidEndDateProvided()
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


            var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _tasksService.Edit(taskDto));
            Assert.That(ex.Message, Is.EqualTo("End date is not in a valid format."));

            await Task.CompletedTask;
        }

        #endregion
    }
}

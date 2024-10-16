﻿using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enumerables;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Repositories.Interfaces;

namespace Application.Services
{
    /// <summary>
    /// Service for managing tasks and their related operations.
    /// Initializes a new instance of the <see cref="TasksService"/> class.
    /// </summary>
    /// <param name="tasksRepository">The tasks repository.</param>
    /// <param name="personsRepository">The persons repository.</param>
    /// <param name="messageSender">The message sender.</param>
    public class TasksService(
        IRedisService redisService,
        ITasksRepository tasksRepository,
        IPersonsRepository personsRepository,
        IMessageSender messageSender) : ITasksService
    {
        #region Properties

        /// <summary>
        /// Instance of the Redis service used to manage caching operations.
        /// </summary>
        private readonly IRedisService _redisService = redisService;

        /// <summary>
        /// Repository for tasks.
        /// </summary>
        private readonly ITasksRepository _tasksRepository = tasksRepository ?? throw new ArgumentNullException(nameof(tasksRepository));

        /// <summary>
        /// Repository for persons.
        /// </summary>
        private readonly IPersonsRepository _personsRepository = personsRepository ?? throw new ArgumentNullException(nameof(personsRepository));

        /// <summary>
        /// Message sender for RabbitMQ.
        /// </summary>
        private readonly IMessageSender _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async Task<List<TasksDTO>> GetById(List<int> ids)
        {
            #region Validations

            if (ids.Any(id => id <= 0)) throw new ArgumentException("All task IDs must be greater than zero.", nameof(ids));

            #endregion

            // Retrieves all tasks if no specific IDs are provided.
            if (ids is null || ids.Count == 0)
            {
                return (await _tasksRepository
                    .GetAllAsync())
                    .Select(x => new TasksDTO
                    {
                        Id = x.Id,
                        PersonId = x.PersonId,
                        Person = x.Person is not null ? new PersonsDTO
                        {
                            Id = x.Person.Id,
                            Name = x.Person.Name
                        } : null,
                        Title = x.Title,
                        Description = x.Description,
                        Location = x.Location,
                        Severity = x.Severity,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate
                    })
                    .ToList();
            }

            // Retrieves tasks of the specific IDs provided from Redis.
            return (await _redisService
                .GetTaskByIdsAsync(ids))
                .Select(x => new TasksDTO
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    Person = x.Person is not null ? new PersonsDTO
                    {
                        Id = x.Person.Id,
                        Name = x.Person.Name
                    } : null,
                    Title = x.Title,
                    Description = x.Description,
                    Location = x.Location,
                    Severity = x.Severity,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                })
                .ToList();
        }

        /// <inheritdoc />
        public async Task<List<PersonTasksDTO>> GetByPersonId(List<int> ids)
        {
            #region Validations

            if (ids.Any(id => id <= 0)) throw new ArgumentException("All person IDs must be greater than zero.", nameof(ids));

            #endregion

            // Retrieves tasks from all persons if no specific person IDs are provided.
            if (ids is null || ids.Count == 0)
            {
                return (await _personsRepository
                    .GetAllAsync())
                    .Select(x => new PersonTasksDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Tasks = x.Tasks?.Select(y => new TasksDTO
                        {
                            Id = y.Id,
                            PersonId = y.PersonId,
                            Title = y.Title,
                            Description = y.Description,
                            Location = y.Location,
                            Severity = y.Severity,
                            StartDate = y.StartDate,
                            EndDate = y.EndDate
                        }).ToList(),
                    })
                    .ToList();
            }

            // Retrieves tasks from the persons of the specific person IDs provided from Redis.
            return (await _redisService
                .GetPersonsByIdsAsync(ids))
                .Select(x => new PersonTasksDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Tasks = x.Tasks?.Select(y => new TasksDTO
                    {
                        Id = y.Id,
                        PersonId = y.PersonId,
                        Title = y.Title,
                        Description = y.Description,
                        Location = y.Location,
                        Severity = y.Severity,
                        StartDate = y.StartDate,
                        EndDate = y.EndDate
                    }).ToList(),
                })
                .ToList();
        }

        /// <inheritdoc />
        public async Task Insert(TasksDTO tasksDto)
        {
            #region Validations

            await ValidateTasksDTO(tasksDto);

            #endregion

            // Sends a message to RabbitMQ queue to save the new task.
            _messageSender.SendMessage(tasksDto);

            // Clear the cached values for the person and associated tasks.
            await _redisService.RemoveCachedValue($"person:{tasksDto.PersonId}");
        }

        /// <inheritdoc />
        public async Task<TasksDTO> Edit(TasksDTO tasksDto)
        {
            #region Validations

            await ValidateTasksDTO(tasksDto);

            #endregion

            // Detach the existing tracked entity if necessary
            var existingPerson = (await _personsRepository.GetByIdsAsync([tasksDto.PersonId])).First();
            if (existingPerson != null)
            {
                await _personsRepository.Detach(existingPerson);
            }

            var person = (await _personsRepository.GetByIdsAsync([tasksDto.PersonId]))
                .FirstOrDefault();

            // Retrieves the task entity.
            var task = (await _redisService
                .GetTaskByIdsAsync([tasksDto.Id]))
                .FirstOrDefault() ?? throw new ArgumentException(null, nameof(tasksDto));

            var oldTaskPersonId = task.PersonId;

            // Updates the task entity.
            task.Title = tasksDto.Title;
            task.PersonId = tasksDto.PersonId;
            task.Person = person;
            task.Description = tasksDto.Description;
            task.Location = tasksDto.Location;
            task.Severity = tasksDto.Severity;
            task.StartDate = tasksDto.StartDate;
            task.EndDate = tasksDto.EndDate;

            tasksDto.Person = person is not null ? new PersonsDTO
            {
                Id = person.Id,
                Name = person.Name
            } : null;

            // Detach the existing tracked entity if necessary
            var existingTask = (await _tasksRepository.GetByIdsAsync([tasksDto.Id])).First();
            if (existingTask != null)
            {
                await _tasksRepository.Detach(existingTask);
            }

            // Saves the task entity.
            await _tasksRepository.UpdateAsync(task);

            // Clear the cached values for the task and associated persons.
            await _redisService.RemoveCachedValue($"task:{tasksDto.Id}");
            await _redisService.RemoveCachedValue($"person:{oldTaskPersonId}");
            await _redisService.RemoveCachedValue($"person:{tasksDto.PersonId}");

            return tasksDto;
        }

        /// <inheritdoc />
        public async Task<TasksDTO> Delete(int id)
        {
            #region Validations

            if (id <= 0) throw new ArgumentException("Task ID must be greater than zero.", nameof(id));

            #endregion

            // Retrieves the task entity.
            var task = (await _redisService
                .GetTaskByIdsAsync([id]))
                .FirstOrDefault() ?? throw new ArgumentException($"No task found with ID {id}.", nameof(id));

            var dto = new TasksDTO
            {
                Id = task.Id,
                PersonId = task.PersonId,
                Person = task.Person is not null ? new PersonsDTO
                {
                    Id = task.Person.Id,
                    Name = task.Person.Name
                } : null,
                Title = task.Title,
                Description = task.Description,
                Location = task.Location,
                Severity = task.Severity,
                StartDate = task.StartDate,
                EndDate = task.EndDate
            };

            // Deletes the task entity.
            await _tasksRepository.DeleteAsync(task);

            // Clear the cached values for the task and associated person.
            await _redisService.RemoveCachedValue($"task:{dto.Id}");
            await _redisService.RemoveCachedValue($"person:{dto.PersonId}");

            return dto;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Validates the provided <see cref="TasksDTO"/> object for any inconsistencies.
        /// Checks if the person ID exists, ensures required fields are populated, 
        /// and validates the task's severity and date formats.
        /// </summary>
        /// <param name="tasksDto">The DTO containing task information to validate.</param>
        /// <exception cref="ArgumentException">Thrown when validation fails for any field.</exception>
        protected async Task ValidateTasksDTO(TasksDTO tasksDto)
        {
            ArgumentNullException.ThrowIfNull(tasksDto, nameof(tasksDto));

            // Validates if a person with the provided ID exists.
            if ((await _redisService
                .GetPersonsByIdsAsync([tasksDto.PersonId]))
                .FirstOrDefault() is null) throw new ArgumentException($"No person found with ID {tasksDto.PersonId}.", nameof(tasksDto.PersonId));

            // Check if the task title is provided and not empty.
            if (string.IsNullOrEmpty(tasksDto.Title)) throw new ArgumentException("Title cannot be null or empty.", nameof(tasksDto.Title));

            // Validate if the provided severity is a defined value in the TaskSeverityEnum.
            if (!Enum.IsDefined(typeof(TaskSeverityEnum), tasksDto.Severity)) throw new ArgumentException($"Severity '{tasksDto.Severity}' is not a valid value.", nameof(tasksDto.Severity));

            // Validate if the StartDate is in a valid format.
            if (!DateTime.TryParse(tasksDto.StartDate.ToString(), out _) || tasksDto.StartDate < DateTime.Now) throw new ArgumentException("Start date is not in a valid format.");

            // Validate if the EndDate is in a valid format.
            if (tasksDto.EndDate is not null && !DateTime.TryParse(tasksDto.EndDate.ToString(), out _)) throw new ArgumentException("End date is not in a valid format.");
        }

        #endregion
    }
}

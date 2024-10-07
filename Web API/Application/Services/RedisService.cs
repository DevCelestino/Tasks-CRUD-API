using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Services
{
    /// <summary>
    /// Service for interacting with Redis and retrieving data from the repository.
    /// </summary>
    /// <param name="redis">The Redis connection multiplexer for caching operations.</param>
    /// <param name="tasksRepository">The repository for accessing task entities from the database.</param>
    public class RedisService(
        IServiceProvider serviceProvider,
        IConnectionMultiplexer redis) : IRedisService
    {
        #region Properties

        /// <summary>
        /// Service provider for resolving dependencies, including repositories.
        /// </summary>
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        /// <summary>
        /// Redis connection multiplexer instance for managing connections to the Redis server.
        /// </summary>
        private readonly IConnectionMultiplexer _redis = redis;

        /// <summary>
        /// Database instance for executing Redis commands.
        /// </summary>
        private readonly IDatabase _database = redis.GetDatabase();

        #endregion

        /// <inheritdoc />
        public async Task<List<TasksEntity>> GetTaskByIdsAsync(List<int> ids)
        {
            var tasks = new List<TasksEntity>();

            foreach (var id in ids)
            {
                string cacheKey = $"task:{id}";

                // Tries to get the cached value from Redis for the specified key.
                var cachedValue = await _database.StringGetAsync(cacheKey);
                if (cachedValue.HasValue)
                {
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve
                    };

                    var cachedTask = JsonSerializer.Deserialize<TasksEntity>(cachedValue, options);
                    if (cachedTask != null)
                    {
                        tasks.Add(cachedTask);
                        continue;
                    }
                }

                // If the value is not found in Redis, retrieve from the repository and cache it.
                using (var scope = _serviceProvider.CreateScope())
                {
                    var tasksRepository = scope.ServiceProvider.GetRequiredService<ITasksRepository>();

                    var tasksFromDb = await tasksRepository.GetByIdsAsync(new List<int> { id });

                    if (tasksFromDb != null && tasksFromDb.Count != 0)
                    {
                        var taskFromDb = tasksFromDb.First();
                        tasks.Add(taskFromDb);

                        var options = new JsonSerializerOptions
                        {
                            ReferenceHandler = ReferenceHandler.Preserve,
                            WriteIndented = true
                        };

                        await _database.StringSetAsync(cacheKey,
                            JsonSerializer.Serialize(taskFromDb, options),
                            TimeSpan.FromMinutes(10));
                    }
                }
            }

            return tasks;
        }

        /// <inheritdoc />
        public async Task<List<PersonsEntity>> GetPersonsByIdsAsync(List<int> ids)
        {
            var persons = new List<PersonsEntity>();

            foreach (var id in ids)
            {
                string cacheKey = $"person:{id}";

                // Tries to get the cached value from Redis for the specified key.
                var cachedValue = await _database.StringGetAsync(cacheKey);
                if (cachedValue.HasValue)
                {
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve
                    };

                    var cachedTask = JsonSerializer.Deserialize<PersonsEntity>(cachedValue, options);
                    if (cachedTask != null)
                    {
                        persons.Add(cachedTask);
                        continue;
                    }
                }

                // If the value is not found in Redis, retrieve from the repository and cache it.
                using (var scope = _serviceProvider.CreateScope())
                {
                    var personsRepository = scope.ServiceProvider.GetRequiredService<IPersonsRepository>();

                    var personsFromDb = (await personsRepository.GetByIdsAsync(new List<int> { id })).ToList();
                    if (personsFromDb != null && personsFromDb.Count != 0)
                    {
                        var personFromDb = personsFromDb.First();
                        persons.Add(personFromDb);

                        var options = new JsonSerializerOptions
                        {
                            ReferenceHandler = ReferenceHandler.Preserve,
                            WriteIndented = true
                        };

                        await _database.StringSetAsync(cacheKey,
                            JsonSerializer.Serialize(personFromDb, options),
                            TimeSpan.FromMinutes(10));
                    }
                }
            }

            return persons;
        }

        /// <inheritdoc />
        public async Task RemoveCachedValue(string cacheKey)
        {
            // Remove the specified cache entry.
            await _database.KeyDeleteAsync(cacheKey);
        }
    }
}
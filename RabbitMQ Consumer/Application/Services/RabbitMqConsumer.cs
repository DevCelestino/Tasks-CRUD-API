using Application.DTOs;
using Domain.Entities;
using Infrastructure.Configurations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Application.Services
{
    /// <summary>
    /// A background service that consumes messages from a RabbitMQ queue and saves tasks to the repository.
    /// </summary>
    public class RabbitMqConsumer : BackgroundService
    {
        #region Properties

        /// <summary>
        /// Service provider for resolving dependencies, including repositories.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Configuration settings for connecting to RabbitMQ.
        /// </summary>
        private readonly RabbitMQSetting _rabbitMqSetting;

        /// <summary>
        /// The RabbitMQ connection used to communicate with the message broker.
        /// </summary>
        private IConnection _connection;

        /// <summary>
        /// The channel through which messages are sent and received from RabbitMQ.
        /// </summary>
        private IModel _channel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConsumer"/> class.
        /// </summary>
        /// <param name="rabbitMqSetting">RabbitMQ configuration settings.</param>
        /// <param name="serviceProvider">Service provider for dependency injection.</param>
        /// <param name="logger">Logger for logging messages.</param>
        public RabbitMqConsumer(
            IOptions<RabbitMQSetting> rabbitMqSetting,
            IServiceProvider serviceProvider)
        {
            _rabbitMqSetting = rabbitMqSetting.Value;
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqSetting.HostName,
                Port = _rabbitMqSetting.Port,
                UserName = _rabbitMqSetting.UserName,
                Password = _rabbitMqSetting.Password
            };

            // Establishes a connection to RabbitMQ.
            while (true)
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    Console.WriteLine("Successfully connected to RabbitMQ!");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to RabbitMQ: {ex.Message}. Trying again in 5 seconds...");
                    Thread.Sleep(5000);
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel.QueueDeclare(queue: "taskQueue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            // Listen for messages from the RabbitMQ queue and saves the task to the repository
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message: {0}", message);

                try
                {
                    var taskDto = JsonSerializer.Deserialize<TasksDTO>(message);

                    if (taskDto != null)
                    {
                        await SaveTask(taskDto);

                        Console.WriteLine("Task saved successfully.");
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {
                        Console.WriteLine("Failed to deserialize message.");
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: "taskQueue", autoAck: false, consumer: consumer);

            await Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Saves a task to the repository.
        /// </summary>
        /// <param name="taskDto">The task data transfer object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SaveTask(TasksDTO taskDto)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tasksRepository = scope.ServiceProvider.GetRequiredService<ITasksRepository>();

                await tasksRepository.AddAsync(new TasksEntity
                {
                    Id = 0,
                    Title = taskDto.Title,
                    PersonId = taskDto.PersonId,
                    Description = taskDto.Description,
                    Location = taskDto.Location,
                    Severity = taskDto.Severity,
                    StartDate = taskDto.StartDate,
                    EndDate = taskDto.EndDate
                });
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }

        #endregion
    }
}

using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Infraestructure.Configurations
{
    /// <summary>
    /// Configuration class for setting up RabbitMQ connections and channels.
    /// </summary>
    public class RabbitMqConfig
    {
        #region Properties

        /// <summary>
        /// Gets the RabbitMQ connection.
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Gets the RabbitMQ channel for communication.
        /// </summary>
        public IModel Channel { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConfig"/> class using the specified configuration.
        /// </summary>
        /// <param name="config">The configuration section containing RabbitMQ settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when the configuration section is null.</exception>
        public RabbitMqConfig(IConfigurationSection config)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var factory = new ConnectionFactory()
            {
                HostName = config["HostName"],
                Port = int.Parse(config["Port"]),
                UserName = config["UserName"],
                Password = config["Password"]
            };
#pragma warning restore CS8604 // Possible null reference argument.

            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();

            Channel.QueueDeclare(queue: "taskQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        #endregion
    }
}

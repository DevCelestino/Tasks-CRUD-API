using Infraestructure.Configurations;
using Infrastructure.Messaging.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infraestructure.Messaging
{
    /// <summary>
    /// Responsible for sending messages to a RabbitMQ queue.
    /// Initializes a new instance of the <see cref="RabbitMqMessageSender"/> class.
    /// </summary>
    /// <param name="rabbitMqConfig">The RabbitMQ configuration.</param>
    public class RabbitMqMessageSender(RabbitMqConfig rabbitMqConfig) : IMessageSender
    {
        #region Properties

        /// <summary>
        /// The RabbitMQ configuration used for message sending.
        /// </summary>
        private readonly RabbitMqConfig _rabbitMqConfig = rabbitMqConfig;

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends a message to the specified RabbitMQ queue.
        /// </summary>
        /// <typeparam name="T">The type of the message to send.</typeparam>
        /// <param name="message">The message to be sent.</param>
        public void SendMessage<T>(T message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _rabbitMqConfig.Channel.BasicPublish(exchange: "",
                                                 routingKey: "taskQueue",
                                                 basicProperties: null,
                                                 body: body);
        }

        #endregion
    }
}

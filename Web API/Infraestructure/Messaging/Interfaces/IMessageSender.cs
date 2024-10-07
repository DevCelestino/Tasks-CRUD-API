namespace Infrastructure.Messaging.Interfaces
{
    /// <summary>
    /// Defines a contract for sending messages to a messaging system.
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Sends a message to the messaging system.
        /// </summary>
        /// <typeparam name="T">The type of the message to send.</typeparam>
        /// <param name="message">The message to be sent.</param>
        void SendMessage<T>(T message);
    }
}

namespace Infrastructure.Configurations
{
    /// <summary>
    /// Represents the configuration settings for connecting to RabbitMQ.
    /// </summary>
    public class RabbitMQSetting
    {
        #region Properties

        /// <summary>
        /// Gets or sets the hostname of the RabbitMQ server.
        /// </summary>
        public string? HostName { get; set; }

        /// <summary>
        /// Gets or sets the port number for the RabbitMQ server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the username for authenticating with RabbitMQ.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the password for authenticating with RabbitMQ.
        /// </summary>
        public string? Password { get; set; }

        #endregion
    }
}

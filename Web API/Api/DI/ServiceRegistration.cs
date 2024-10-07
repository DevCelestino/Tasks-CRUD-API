using Application.Services;
using Application.Services.Interfaces;
using Infrastructure.Messaging.Interfaces;
using Infraestructure.Messaging;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Repositories;

namespace Api.DI
{
    /// <summary>
    /// Provides extension methods for registering services in the dependency injection container.
    /// </summary>
    public static class ServiceRegistration
    {
        #region Public Methods

        /// <summary>
        /// Adds application services to the specified IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <returns>The updated IServiceCollection.</returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Register application services with the DI container
            #region Services

            services.AddScoped<ITasksService, TasksService>();
            services.AddScoped<IMessageSender, RabbitMqMessageSender>();

            #endregion

            // Register repositories with the DI container
            #region Repositories

            services.AddScoped<ITasksRepository, TasksRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();

            #endregion

            return services;
        }

        #endregion
    }
}

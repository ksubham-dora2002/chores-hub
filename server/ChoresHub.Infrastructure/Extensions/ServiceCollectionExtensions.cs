using ChoresHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ChoresHub.Infrastructure.Contexts;
using Microsoft.Extensions.Configuration;
using ChoresHub.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using ChoresHub.Infrastructure.Services;
using ChoresHub.Application.Interfaces;

namespace ChoresHub.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ChoresHubDb");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection string 'ChoresHubDb' is null or empty.");
            }
            services.AddDbContext<PostgreDbContext>(options =>
                options.UseNpgsql(configuration?.GetConnectionString("ChoresHubDb")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IShoppingProductRepository, ShoppingProductRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();

            return services;
        }
    }
}
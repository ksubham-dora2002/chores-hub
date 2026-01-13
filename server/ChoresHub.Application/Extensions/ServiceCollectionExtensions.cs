using System.Reflection;
using ChoresHub.Application.Helpers;
using ChoresHub.Application.Services;
using ChoresHub.Application.Validators;
using ChoresHub.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ChoresHub.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            
            var applicationAssembly = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(applicationAssembly);

            services.AddScoped<CloudinaryHelper>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserValidators, UserValidators>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IShoppingProductService, ShoppingProductService>();

            return services;
        }
    }
}
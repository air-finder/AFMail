using System.Diagnostics.CodeAnalysis;
using Application;
using Application.Mails;
using Domain.SeedWork.Notification;
using Infra.Data;
using Infra.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.IoC
{
    [ExcludeFromCodeCoverage]
    public static class NativeInjector
    {
        public static void AddLocalHttpClients(this IServiceCollection services, IConfiguration configuration) {}

        public static void AddLocalServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Services
            services.AddHttpContextAccessor();
            services.AddScoped<INotification, Notification>();
            services.AddScoped<IMailService, MailService>();
            services.AddSingleton<IContainer, ServiceProviderProxy>();
            #endregion
        }

        public static void AddLocalUnitOfWork(this IServiceCollection services, IConfiguration configuration)
        {
            var connString = Builders.BuildConnectionString(configuration);
            services.AddDbContext<Context>(options => options.UseLazyLoadingProxies().UseSqlServer(connString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}

using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilentMoon.Application.Common.Services;
using SilentMoon.Application.Interfaces.Caching;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.Application.Common.Services;
using SilentMoon.Application.Interfaces.Repositories;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Caching;
using SilentMoon.Infrastructure.Persistence.Contexts;
using SilentMoon.Infrastructure.Persistence.Dapper;
using SilentMoon.Infrastructure.Persistence.Logging;
using SilentMoon.Infrastructure.Persistence.Messaging;
using SilentMoon.Infrastructure.Persistence.Repositories;
using SilentMoon.Infrastructure.Persistence.Services;
using SilentMoon.Infrastructure.Persistence.Settings;
using System;
using System.Linq;
using System.Reflection;

namespace SilentMoon.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseOracle(configuration["APIAppSettings:ConnectionString"],
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
            services.AddHostedService<OtpEmailConsumer>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["APIAppSettings:Redis"];
                options.InstanceName = Assembly.GetEntryAssembly()?.GetName().Name + "_";
            });

            services.Configure<APIAppSettings>(configuration.GetSection("APIAppSettings"));

            services.AddSingleton(typeof(IAppLogger<>), typeof(LoggerManager<>));
            services.AddScoped<ICacheService, RedisCacheService>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<IDapper, DapperClass>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IAuthTokenIssuer, AuthTokenIssuer>();
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
            services.AddScoped<IOtpDispatcher, OtpDispatcher>();

            services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();
            services.AddHttpClient<IFacebookAuthService, FacebookAuthService>();

            services.AddScoped<IPomodoroRepository, PomodoroRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            services.AddScoped<IReminderRepository, ReminderRepository>();

            services.AddScoped<IUow, Uow>();

            RegisterDapperDomainMappings();
        }

        #region APIRepositories
        public static void AddPersistenceApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }
        #endregion

        private static void RegisterDapperDomainMappings()
        {
            var domainAssembly = Assembly.Load("SilentMoon.Domain");
            var entityTypes = domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace != null && t.Namespace.EndsWith("Entities"));

            foreach (var type in entityTypes)
            {
                var mapperType = typeof(ColumnAttributeTypeMapper<>).MakeGenericType(type);
                var mapper = (SqlMapper.ITypeMap)Activator.CreateInstance(mapperType);
                SqlMapper.SetTypeMap(type, mapper);
            }
        }
    }
}
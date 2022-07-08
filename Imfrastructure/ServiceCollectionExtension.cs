using Application.DatingApp.Interface;
using Domain.Configuration;
using Infrastructure.Persistance;
using Infrastructure.Persistance.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // strongly type configurations settings (have to Bind it in the dependency injetion class)
            services.Configure<CloudinarySettings>(x => configuration.GetSection("CloudinarySettings").Bind(x));
            services.AddDbContext<DataContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DatabaseCS"),
                b => b.MigrationsAssembly(typeof(DataContext).Assembly.FullName)), ServiceLifetime.Transient
            );
            //services.AddDbContext<DataContext>(options => 
            // options.UseSqlServer(configuration.GetConnectionString("DatabaseCS"), 
            // b => b.MigrationsAssembly(typeof(DataContext).Assembly.FullName)), ServiceLifetime.Transient
            // );
            services.AddScoped<IDbContext>(provider => provider.GetService<DataContext>());
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();            
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            return services;
        }
    }
}

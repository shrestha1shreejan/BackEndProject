using Application.Common.Interface;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options => 
                options.UseSqlite(configuration.GetConnectionString("DatabaseCS"), 
                b => b.MigrationsAssembly(typeof(DataContext).Assembly.FullName)), ServiceLifetime.Transient
            );
            //services.AddDbContext<DataContext>(options => 
            // options.UseSqlServer(configuration.GetConnectionString("DatabaseCS"), 
            // b => b.MigrationsAssembly(typeof(DataContext).Assembly.FullName)), ServiceLifetime.Transient
            // );
            services.AddScoped<IDbContext>(provider => provider.GetService<DataContext>());
            return services;
        }
    }
}

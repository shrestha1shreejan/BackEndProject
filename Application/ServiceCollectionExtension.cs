using Application.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<DataContext>(options =>
            //    options.UseSqlite(configuration.GetConnectionString("DatabaseCS"),
            //    b => b.MigrationsAssembly(typeof(DataContext).Assembly.FullName)), ServiceLifetime.Transient
            //);
            //services.AddDbContext<DataContext>(options => 
            // options.UseSqlServer(configuration.GetConnectionString("DatabaseCS"), 
            // b => b.MigrationsAssembly(typeof(DataContext).Assembly.FullName)), ServiceLifetime.Transient
            // );
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}

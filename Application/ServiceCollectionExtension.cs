using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //services.AddDbContext<DataContext>(options =>
            //    options.UseSqlite(configuration.GetConnectionString("DatabaseCS"),
            //    b => b.MigrationsAssembly(typeof(DataContext).Assembly.FullName)), ServiceLifetime.Transient
            //);
            //services.AddDbContext<DataContext>(options => 
            // options.UseSqlServer(configuration.GetConnectionString("DatabaseCS"), 
            // b => b.MigrationsAssembly(typeof(DataContext).Assembly.FullName)), ServiceLifetime.Transient
            // );
            return services;
        }
    }
}

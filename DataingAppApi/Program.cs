using Application;
using DataingAppApi.Extensions;
using Infrastructure;
using Application.Common.ErrorHandling;
using Infrastructure.Persistance;
using Application.Common.Interface;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(c => {
    c.AddPolicy("CustomPolicy", options => {
        options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// adding library by dependecy injection
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);

var app = builder.Build();



app.UseMiddleware<ExceptionsMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CustomPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// adding seed
var scopeFactory = app.Services.GetService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var service = scope.ServiceProvider;
try
{
    var context = service.GetRequiredService<IDbContext>();
    await context.MigrateAsync();
    await Seed.SeedUser(context);
}
catch (Exception exception)
{
    var logger = service.GetRequiredService<ILogger<Program>>();
    logger.LogError(exception, "exception occured during migration");
}
await app.RunAsync();
//

app.Run();

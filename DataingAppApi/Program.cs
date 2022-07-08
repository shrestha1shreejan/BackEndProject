using Application;
using DataingAppApi.Extensions;
using Infrastructure;
using Application.Common.ErrorHandling;
using Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;
using Domain.DatingSite;
using Microsoft.EntityFrameworkCore;
using Domain.Common.Auth.IdentityAuth;
using DataingAppApi.SignalROperations;

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
    c.AddPolicy("SignalRPolicy", options => {
        options.AllowCredentials().AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(origin => true);
    });
});

// adding library by dependecy injection
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddSingleton<PresenceTracker>();

var app = builder.Build();



app.UseMiddleware<ExceptionsMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseCors("CustomPolicy");
app.UseCors("SignalRPolicy");


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

// adding seed
var scopeFactory = app.Services.GetService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var service = scope.ServiceProvider;
try
{
    var context = service.GetRequiredService<DataContext>();
    var userManager = service.GetRequiredService<UserManager<AppUser>>();
    var roleManager = service.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    // await Seed.SeedUser(userManager, roleManager);
}
catch (Exception exception)
{
    var logger = service.GetRequiredService<ILogger<Program>>();
    logger.LogError(exception, "exception occured during migration");
}
await app.RunAsync();
//

app.Run();

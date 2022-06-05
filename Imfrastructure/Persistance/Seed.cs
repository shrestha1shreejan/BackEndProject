using Application.Common.Interface;
using Domain.DatingSite;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Persistance
{
    public class Seed
    {
        public static async Task SeedUser(IDbContext context)
        {
            if (await context.Users.AnyAsync())
            {
                return;
            }
            
            var userData = await File.ReadAllTextAsync("SeedFiles/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);
            }

            await context.SaveChangesAsync();
        }
    }
}

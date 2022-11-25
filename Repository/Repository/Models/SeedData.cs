using Microsoft.EntityFrameworkCore;
using System.Web.Helpers;

namespace Repository.Models;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new UserContext(
            serviceProvider.GetRequiredService<DbContextOptions<UserContext>>()))
        {
            // Look for any users
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }
            context.Users.AddRange(
                new User
                {
                    FullName = "janez",
                    UserName = "Janez Novak",
                    Email = "janez@email.si",
                    MobileNumber = "040111222",
                    Language = "slovenian",
                    Culture = "art",
                    Password = Crypto.HashPassword("geslo")
                },
                new User
                {
                    FullName = "marko",
                    UserName = "Marko Horvat",
                    Email = "mare@gmail.com",
                    MobileNumber = "+386 30123456",
                    Language = "slovenian",
                    Culture = "music",
                    Password = Crypto.HashPassword("markomarko")
                },
                new User
                {
                    FullName = "johnny",
                    UserName = "John Doe",
                    Email = "johnny@hotmail.com",
                    MobileNumber = "020 7946 0000",
                    Language = "english",
                    Culture = "film",
                    Password = Crypto.HashPassword("secret")
                },
                new User
                {
                    FullName = "hans",
                    UserName = "Hans Zimmer",
                    Email = "hans@example.com",
                    MobileNumber = "+49 171 1234567",
                    Language = "german",
                    Culture = "food",
                    Password = Crypto.HashPassword("berliner")
                }
            );
            context.SaveChanges();
        }
    }
}

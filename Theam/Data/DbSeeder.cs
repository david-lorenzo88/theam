using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theam.API.Models;
using Theam.API.Utils;

namespace Theam.API.Data
{
    /// <summary>
    /// Class responsible of initializating database the first time
    /// with initial data
    /// </summary>
    public static class DbSeeder
    {
        /// <summary>
        /// Initializes the database the first time with initial data
        /// </summary>
        public static async Task Initialize(ApiContext context)
        {
            await context.Database.MigrateAsync();

            if (!context.Roles.Any())
            {
                await context.Roles.AddAsync(new Role
                {
                    Name = Constants.ROLE_ADMIN
                });
                await context.Roles.AddAsync(new Role
                {
                    Name = Constants.ROLE_USER
                });

                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {

                var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == Constants.ROLE_ADMIN);
                var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == Constants.ROLE_USER);

                await context.Users.AddAsync(new User
                {
                    Name = "David Admin",
                    Surname = "Lorenzo",
                    Password = PasswordHasherHelper.ComputePassword("David123"),
                    Email = "admin@test.com",
                    Roles = new List<UserRole>
                    {
                        new UserRole {Role = adminRole }
                    }
                });


                await context.Users.AddAsync(new User
                {
                    Name = "David User",
                    Surname = "Lorenzo",
                    Password = PasswordHasherHelper.ComputePassword("David123"),
                    Email = "user@test.com",
                    Roles = new List<UserRole>
                    {
                        new UserRole{Role = userRole }
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}

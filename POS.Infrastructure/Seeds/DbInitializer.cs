using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using POS.Domain.Constants;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Infrastructure.Seeds
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            await context.Database.MigrateAsync();

            var allRoles = new List<AppRole>
        {
            new AppRole { Name = RolesConstant.SYSTEM_MANAGEMENT, Group = RoleGroup.SYSTEM_MANAGEMENT },
            new AppRole { Name = RolesConstant.SALES_ADD, Group = RoleGroup.SALES },
            new AppRole { Name = RolesConstant.BOX_REPORT, Group = RoleGroup.BOX_OFFICE },
            new AppRole { Name = RolesConstant.BOX_EXPENSE, Group = RoleGroup.BOX_OFFICE },
            new AppRole { Name = RolesConstant.CUSTOMERS_STATEMENT, Group = RoleGroup.CUSTOMERS },
            new AppRole { Name = RolesConstant.USERS_VIEW, Group = RoleGroup.USERS }
        };

            foreach (var role in allRoles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                    await roleManager.CreateAsync(role);
            }

            if (!await userManager.Users.AnyAsync(u => u.UserName == "حماده"))
            {
                var user = new AppUser
                {
                    FullName = "احمد باكحيل",
                    UserName = "حماده",
                    Email = "ddbp@hadhramout.com",
                    EmailConfirmed = true,
                    IsActive = true
                };
                var result = await userManager.CreateAsync(user, "ddbp@123#");
                if (result.Succeeded)
                {
                    var roles = allRoles.Select(r => r.Name).ToArray();
                    await userManager.AddToRolesAsync(user, roles);
                }
            }

            if (!await userManager.Users.AnyAsync(u => u.UserName == "superadmin"))
            {
                var admin = new AppUser
                {
                    FullName = "مدير النظام",
                    UserName = "superadmin",
                    Email = "superadmin@hadhramout.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "123456");
                if (result.Succeeded)
                {
                    var roles = allRoles.Select(r => r.Name).ToArray();
                    await userManager.AddToRolesAsync(admin, roles);
                }
            }
        }
    }
}

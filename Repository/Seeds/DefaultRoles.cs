using System.Threading.Tasks;
using Core.Entity;
using Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace Repository.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            var isAdminExist = await roleManager.FindByNameAsync(Roles.Admin.ToString());
            if (isAdminExist is null)
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            }
            var isUserExist = await roleManager.FindByNameAsync(Roles.User.ToString());
            if (isUserExist is null)
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
            }
        }
    }
}

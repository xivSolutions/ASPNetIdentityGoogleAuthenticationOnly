using ASPNetIdentityGoogleAuthenticationOnly.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Web;

namespace ASPNetIdentityGoogleAuthenticationOnly.IdentityConfig
{
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var currentContext = HttpContext.Current;

            var ctx = currentContext.GetOwinContext();

            var userManager = ctx.GetUserManager<ApplicationUserManager>();
            var roleManager = ctx.Get<ApplicationRoleManager>();

            const string firstName = "Alex";
            const string lastName = "Brown";
            const string email = "alex@cohoda.com";
            const string roleName = "Admin";

            //Create role if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                roleManager.Create(role);
            }

            var user = userManager.FindByEmailAsync(email).Result;

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName
                };
                userManager.Create(user);

                userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);

            if (!rolesForUser.Contains(role.Name))
                userManager.AddToRole(user.Id, role.Name);
        }
    }
}
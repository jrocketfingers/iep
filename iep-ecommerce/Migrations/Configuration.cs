namespace iep_ecommerce.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<iep_ecommerce.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(iep_ecommerce.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var adminRoleName = "admin";

            if (!RoleManager.RoleExists(adminRoleName))
            {
                var roleresult = RoleManager.Create(new IdentityRole(adminRoleName));
            }

            var admin = new ApplicationUser();
            var password = "qwe123";
            admin.UserName = "admin@rocketfinge.rs";
            admin.Email = "admin@rocketfinge.rs";
            var adminResult = UserManager.Create(admin, password);

            if (adminResult.Succeeded)
            {
                var result = UserManager.AddToRole(admin.Id, adminRoleName);
            }
        }
    }
}

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SoniReports.Controllers;
using SoniReports.DomainModel;
using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Web;

namespace SoniReports.DataAccessLayer
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual IDbSet<UserGroup> UserGroups { get; set; }
        public virtual IDbSet<UserGroupAssociation> UserGroupAssociations { get; set; }
        public virtual IDbSet<UserGroupAssociationHistory> UserGroupAssociationHistory { get; set; }
        public virtual IDbSet<UserActivityLogEntry> UserActivityLog { get; set; }

        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new ApplicationDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<UserGroupAssociation>()
            //    .HasRequired(e => e.UserGroup)
            //    .WithMany(e => e.UserGroupAssociations)
            //    .HasForeignKey(e => e.UserGroupId);

            //modelBuilder.Entity<UserGroupAssociation>()
            //    .HasRequired(e => e.User)
            //    .WithMany(e => e.UserGroupAssociations)
            //    .HasForeignKey(e => e.UserGroupId);

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public static T DbAction<T>(Func<ApplicationDbContext, T> action)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                try
                {
                    return action(db);
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var error in ex.EntityValidationErrors)
                    {
                    }
                    throw ex;
                }
            }
        }
    }

    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);

            CreateRoles(context);

            //TODO: FOR TESTING PURPOSES ONLY - Remove in production
            CreateTestUsers();
        }

        private static void CreateTestUsers()
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(); 

            CreateUserWithRoles(userManager, "test-bruger1@freelance-udvikler.dk", "Psd531!", SoniRoles.ReportViewer);
            CreateUserWithRoles(userManager, "test-admin@freelance-udvikler.dk", "Psd531!", SoniRoles.Admin);
            CreateUserWithRoles(userManager, "test-superuser@freelance-udvikler.dk", "Psd531!", SoniRoles.SuperUser);

            var userGroupAccess = new UserGroupAccess();
            userGroupAccess.Create(new UserGroup { Id = Guid.NewGuid().ToString(), Name = "Testgruppe 1", Enabled = true });
            userGroupAccess.Create(new UserGroup { Id = Guid.NewGuid().ToString(), Name = "Testgruppe 2", Enabled = true });
        }

        private static void CreateUserWithRoles(ApplicationUserManager userManager, string nameEmail, string password, params string[] roles)
        {
            var user = new ApplicationUser
            {
                Email = nameEmail,
                UserName = nameEmail,
                EmailConfirmed = true,
                Activated = ActivationStatus.Activated
            };
            var result = userManager.Create(user, password);
            foreach (var role in roles) userManager.AddToRole(user.Id, role);
        }


        private static void CreateRoles(ApplicationDbContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            roleManager.Create(new IdentityRole(SoniRoles.SuperUser));
            roleManager.Create(new IdentityRole(SoniRoles.Admin));
            roleManager.Create(new IdentityRole(SoniRoles.ReportViewer));
        }

    }
}
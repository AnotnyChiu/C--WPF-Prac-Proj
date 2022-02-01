namespace FriendOrganizer.DataAccess.Migrations
{
    using FriendOrganizer.Model;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizer.DataAccess.FriendOrganizerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganizer.DataAccess.FriendOrganizerDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            // 這邊是code first migration，在AddOrUpdate裡面

            // 以Firstname作為判斷，如果存在於DB則Update，如果不再則新增
            context.Friends.AddOrUpdate(
                f => f.FirstName,
                new Friend { FirstName = "Thomas", LastName = "Huber" },
                new Friend { FirstName = "Antony", LastName = "Chiu" },
                new Friend { FirstName = "Kenny", LastName = "Hsiao" },
                new Friend { FirstName = "Frank", LastName = "Wang" }
                );

            // seed fpr programming language
            context.ProgrammingLanguages.AddOrUpdate(pl => pl.Name,
                new ProgrammingLanguage { Name = "C#" },
                new ProgrammingLanguage { Name = "TypeScript" },
                new ProgrammingLanguage { Name = "Swift" },
                new ProgrammingLanguage { Name = "Java" },
                new ProgrammingLanguage { Name = "F#" }
                );
        }
    }
}

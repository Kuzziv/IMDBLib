using IMDBLib.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Services.DatabaseServices
{
    public class DbDeleteService
    {
        public static void DeleteAllData()
        {
            using (var context = new MyDbContext())
            {
                Console.WriteLine("Truncating tables...");

                // Delete data from related tables
                context.Database.ExecuteSqlRaw("DELETE FROM Title_Genres");
                context.Database.ExecuteSqlRaw("DELETE FROM Genres");
                context.Database.ExecuteSqlRaw("DELETE FROM TitleTypes");
                context.Database.ExecuteSqlRaw("DELETE FROM Titles");
                context.Database.ExecuteSqlRaw("DELETE FROM Crews");
                context.Database.ExecuteSqlRaw("DELETE FROM Crew_Professions");
                context.Database.ExecuteSqlRaw("DELETE FROM Directors");
                context.Database.ExecuteSqlRaw("DELETE FROM Know_For_Titles");
                context.Database.ExecuteSqlRaw("DELETE FROM Writers");
                context.Database.ExecuteSqlRaw("DELETE FROM Title_Crews");
                context.Database.ExecuteSqlRaw("DELETE FROM Jobs");


                // Truncate Title table
                //context.Database.ExecuteSqlRaw("TRUNCATE TABLE Title_Genres");
                //context.Database.ExecuteSqlRaw("TRUNCATE TABLE Genres");
                //context.Database.ExecuteSqlRaw("TRUNCATE TABLE TitleTypes");
                //context.Database.ExecuteSqlRaw("TRUNCATE TABLE Titles");
                //context.Database.ExecuteSqlRaw("TRUNCATE TABLE Crews");
                //context.Database.ExecuteSqlRaw("TRUNCATE TABLE CrewProfessions");
                //context.Database.ExecuteSqlRaw("TRUNCATE TABLE Directors");
                //context.Database.ExecuteSqlRaw("TRUNCATE TABLE KnowForTitles");
                //context.Database.ExecuteSqlRaw("TRUNCATE TABLE Writers");
                //context.Database.ExecuteSqlRaw("TRUNCATE TABLE TitleCrews");
                //context.Database.ExecuteSqlRaw("TRUNCATE TABLE Jobs");


                Console.WriteLine("Tables truncated successfully.");
            }
        }
    }
}

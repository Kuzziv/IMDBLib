using System;
using System.Linq;
using System.Threading.Tasks;
using IMDBLib.DataBase;
using Microsoft.EntityFrameworkCore;

namespace IMDBLib.Services
{
    public class DataDeleteService
    {
        private readonly IMDBDbContext _dbContext;

        public DataDeleteService(IMDBDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteAllDataAsync()
        {
            try
            {
                await CleanDatabaseAsync();
                Console.WriteLine("All data deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during data deletion: {ex}");
            }
        }

        private async Task CleanDatabaseAsync()
        {
            Console.WriteLine("Cleaning database...");

            // Remove all title-person relationships
            _dbContext.TitlePersons.RemoveRange(await _dbContext.TitlePersons.ToListAsync());
            Console.WriteLine("TitlePersons cleaned");

            // Remove all titles
            _dbContext.Titles.RemoveRange(await _dbContext.Titles.ToListAsync());
            Console.WriteLine("Titles cleaned");

            // Remove all persons
            _dbContext.Persons.RemoveRange(await _dbContext.Persons.ToListAsync());
            Console.WriteLine("Persons cleaned");

            // Remove all professions
            _dbContext.Professions.RemoveRange(await _dbContext.Professions.ToListAsync());
            Console.WriteLine("Professions cleaned");

            // Remove all person-profession relationships
            _dbContext.PersonProfessions.RemoveRange(await _dbContext.PersonProfessions.ToListAsync());
            Console.WriteLine("PersonProfessions cleaned");

            // Remove all title-genre relationships
            _dbContext.TitleGenres.RemoveRange(await _dbContext.TitleGenres.ToListAsync());
            Console.WriteLine("TitleGenres cleaned");

            // Remove all genres
            _dbContext.Genres.RemoveRange(await _dbContext.Genres.ToListAsync());
            Console.WriteLine("Genres cleaned");

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            Console.WriteLine("Database cleaned successfully");
        }
    }
}

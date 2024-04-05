using IMDBLib.DAO;
using IMDBLib.DataBase;
using IMDBLib.Models.People;
using IMDBLib.Models.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Services.DAOServices
{
    public class PersonService : IPersonService
    {
        private readonly MyDbContext _dbContext;

        public PersonService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddPerson(CrewDAO crewDAO)
        {
            try
            {
                // get the highest Nconst and increment it by 1
                var highestNconst = _dbContext.Crews.Max(c => c.Nconst);
                var newNconst = (int.Parse(highestNconst.Substring(2)) + 1).ToString("nm0000000");

                Crew newCrew = new Crew
                {
                    Nconst = newNconst,
                    PrimaryName = crewDAO.PrimaryName,
                    BirthYear = crewDAO.BirthYear,
                    DeathYear = crewDAO.DeathYear,
                };

                _dbContext.Crews.Add(newCrew);
                _dbContext.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                // Rethrow the exception or return a suitable error response
                throw new Exception("Error in PersonService.AddPerson: Database error.", ex);
            }
            catch (Exception ex)
            {
                // Rethrow the exception or return a suitable error response
                throw new Exception("Error in PersonService.AddPerson: An unexpected error occurred.", ex);
            }
        }

        public Task DeletePerson()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PersonView>> GetAllPersons(int pageNumber, int pageSize)
        {
            try
            {
                // Get all persons
                var query = _dbContext.PersonViews.AsQueryable();
                // Calculate the number of items to skip
                int skip = (pageNumber - 1) * pageSize;
                // Use the extension method to retrieve the data
                var result = await query.Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in PersonService.GetAllPersons", ex);
            }
        }

        public Task<PersonView> GetPersonInfoByNconst(string nconst)
        {
            throw new NotImplementedException();
        }

        public Task<List<PersonView>> GetPersonListByName(string searchString)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePerson()
        {
            throw new NotImplementedException();
        }
    }
}

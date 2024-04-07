using IMDBLib.DataBase;
using IMDBLib.DTO;
using IMDBLib.Models.People;
using IMDBLib.Models.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMDBLib.Services.APIServices
{
    public class PersonService : IPersonService
    {
        private readonly IMDBDbContext _dbContext;
        private readonly ILogger<PersonService> _logger;

        public PersonService(IMDBDbContext dbContext, ILogger<PersonService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger;
        }

        public async Task<bool> AddPersonAsync(PersonDTO person)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                if (person == null)
                    throw new ArgumentNullException(nameof(person), "Person DTO cannot be null.");

                var newPerson = new Person
                {
                    Nconst = await GetNewNconstAsync(),
                    PrimaryName = person.PrimaryName,
                    BirthYear = person.BirthYear,
                    DeathYear = person.DeathYear
                };

                _dbContext.Persons.Add(newPerson);

                foreach (var professionName in person.ProfessionNames)
                {
                    if (string.IsNullOrEmpty(professionName))
                        continue;
                    var existingProfession = await _dbContext.Professions
                        .FirstOrDefaultAsync(p => p.ProfessionName == professionName);

                    if (existingProfession == null)
                    {
                        existingProfession = new Profession { ProfessionName = professionName };
                        _dbContext.Professions.Add(existingProfession);
                    }

                    if (newPerson.PersonProfessions == null)
                    {
                        newPerson.PersonProfessions = new List<PersonProfession>();
                    }

                    newPerson.PersonProfessions.Add(new PersonProfession { Profession = existingProfession });
                }

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, $"An error occurred while adding person: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<PersonDTO>> GetAllPersonsAsync(int page, int pageSize)
        {
            try
            {
                var persons = await _dbContext.PersonViews
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return MapPersonViewToPersonDto(persons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching all persons: {ex.Message}");
                throw;
            }
        }

        public async Task<PersonDTO> GetPersonByNmconstAsync(string nmconst)
        {
            try
            {
                if (string.IsNullOrEmpty(nmconst))
                    throw new ArgumentException("Person Nconst cannot be null or empty.");

                var person = await _dbContext.PersonViews
                    .FirstOrDefaultAsync(p => p.Nconst == nmconst);

                return MapPersonViewToPersonDto(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching person by ID: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<PersonDTO>> SearchPersonsByNameAsync(string searchName, int page, int pageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(searchName))
                    throw new ArgumentException("Search name cannot be null or empty.");

                var persons = await _dbContext.PersonViews
                    .Where(p => p.PrimaryName.Contains(searchName))
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return MapPersonViewToPersonDto(persons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while searching persons by name: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetNewNconstAsync()
        {
            try
            {
                var highestNconst = await _dbContext.Persons
                    .OrderByDescending(p => p.Nconst)
                    .FirstOrDefaultAsync();

                if (highestNconst == null)
                    return "nm0000001"; // Start from nm0000001 if no person exists

                var newNconst = (int.Parse(highestNconst.Nconst.Substring(2)) + 1).ToString();
                return $"nm{newNconst.PadLeft(7, '0')}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while generating new person Nconst: {ex.Message}");
                throw;
            }
        }

        // Map PersonView properties to PersonDTO entity (overload for single object)
        private PersonDTO MapPersonViewToPersonDto(PersonView personView)
        {
            return new PersonDTO
            {
                Nconst = personView.Nconst,
                PrimaryName = personView.PrimaryName,
                BirthYear = personView.BirthYear,
                DeathYear = personView.DeathYear,
                ProfessionNames = personView.ProfessionNames.Split(',').ToList()
            };
        }

        // Map PersonView properties to PersonDTO entity (overload for list)
        private List<PersonDTO> MapPersonViewToPersonDto(List<PersonView> personsView)
        {
            return personsView.Select(p => MapPersonViewToPersonDto(p)).ToList();
        }
    }
}

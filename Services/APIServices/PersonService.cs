﻿using IMDBLib.DataBase;
using IMDBLib.Models.People;
using IMDBLib.Models.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMDBLib.Services.APIServices
{
    public class PersonService : IPersonService
    {
        private readonly IMDBDbContext _dbContext;

        public PersonService(IMDBDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddPersonAsync(Person person)
        {
            try
            {
                _dbContext.Persons.Add(person);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding person: {ex}");
                throw new Exception("Failed to add person.", ex);
            }
        }

        public async Task<IEnumerable<PersonView>> GetAllPersonsAsync(int page, int pageSize)
        {
            try
            {
                var query = _dbContext.PersonViews;

                var persons = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return persons;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching all persons: {ex}");
                throw new Exception("Failed to fetch all persons.", ex);
            }
        }

        public async Task<PersonView> GetPersonByIdAsync(string nmconst)
        {
            try
            {
                var person = await _dbContext.PersonViews
                    .FirstOrDefaultAsync(p => p.Nconst == nmconst);

                return person;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching person by ID: {ex}");
                throw new Exception("Failed to fetch person by ID.", ex);
            }
        }

        public async Task<IEnumerable<PersonView>> SearchPersonsByNameAsync(string searchName, int page, int pageSize)
        {
            try
            {
                var query = _dbContext.PersonViews
                    .Where(p => p.PrimaryName.Contains(searchName));

                var persons = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return persons;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while searching persons by name: {ex}");
                throw new Exception("Failed to search persons by name.", ex);
            }
        }
    }
}
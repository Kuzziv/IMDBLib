using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using IMDBLib.DataBase;
using IMDBLib.Models; // Assuming your entity classes are in this namespace
using IMDBLib.Models.Movie;
using IMDBLib.Models.People;
using Microsoft.EntityFrameworkCore;

namespace IMDBLib.Services
{
    public class DataImportService
    {
        private readonly YourDbContext _dbContext;

        public DataImportService(YourDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ImportData(string titleFilePath, string nameFilePath, string titleCrewFilePath, int maxRecords = int.MaxValue)
        {
            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    // Read and import data from title.basic file
                    ImportTitlesFromFile(titleFilePath, maxRecords);

                    // Read and import data from name.basic file
                    ImportPersonsFromFile(nameFilePath, maxRecords);

                    // Read and import data from title.crew file
                    ImportTitleCrewFromFile(titleCrewFilePath, maxRecords);

                    // Save changes to persist imported data
                    _dbContext.SaveChanges();

                    transactionScope.Complete(); // Commit the transaction if everything is successful
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred during data import: \n {ex}");
                }
            }
        }

        private void ImportTitlesFromFile(string filePath, int maxRecords)
        {
            try
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    // Skip header line
                    streamReader.ReadLine();

                    var titlesToAdd = new List<Title>(); // Create a list to store titles
                    int recordsProcessed = 0;

                    while (!streamReader.EndOfStream && recordsProcessed < maxRecords)
                    {
                        var line = streamReader.ReadLine();
                        var values = line.Split('\t');

                        var title = new Title
                        {
                            Tconst = values[0],
                            TitleType = values[1],
                            PrimaryTitle = values[2],
                            OriginalTitle = values[3],
                            IsAdult = values[4] == "1",
                            StartYear = values[5] == @"\N" ? 0 : int.Parse(values[5]),
                            EndYear = values[6] == @"\N" ? null : int.Parse(values[6]),
                            RuntimeMinutes = int.Parse(values[7]),
                            TitleGenres = new List<TitleGenre>(), // Initialize empty list
                            TitlePersons = new List<TitlePerson>() // Initialize empty list
                        };

                        // Parse and add genres
                        var genres = values[8].Split(',');
                        foreach (var genre in genres)
                        {
                            if (!string.IsNullOrEmpty(genre))
                            {
                                int genreId;
                                if (_dbContext.Genres.Any(g => g.GenreName == genre))
                                {
                                    genreId = _dbContext.Genres.First(g => g.GenreName == genre).GenreId;
                                }
                                else
                                {
                                    var newGenre = new Genre { GenreName = genre };
                                    _dbContext.Genres.Add(newGenre);
                                    _dbContext.SaveChanges();
                                    genreId = newGenre.GenreId;
                                }

                                title.TitleGenres.Add(new TitleGenre { Tconst = title.Tconst, GenreId = genreId });
                            }
                        }

                        titlesToAdd.Add(title); // Add title to the list
                        recordsProcessed++;

                        if (titlesToAdd.Count >= 1000) // Insert in batches of 1000
                        {
                            _dbContext.Titles.AddRange(titlesToAdd); // Bulk insert titles
                            _dbContext.SaveChanges(); // Save changes
                            titlesToAdd.Clear(); // Clear the list
                        }
                    }

                    // Insert remaining titles
                    if (titlesToAdd.Any())
                    {
                        _dbContext.Titles.AddRange(titlesToAdd);
                        _dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during import of titles: \n {ex}");
            }
        }



        private void ImportPersonsFromFile(string filePath, int maxRecords)
        {
            try
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    // Skip header line
                    streamReader.ReadLine();

                    var personsToAdd = new List<Person>(); // Create a list to store persons
                    int recordsProcessed = 0;

                    while (!streamReader.EndOfStream && recordsProcessed < maxRecords)
                    {
                        var line = streamReader.ReadLine();
                        var values = line.Split('\t');

                        var person = new Person
                        {
                            Nconst = values[0],
                            PrimaryName = values[1],
                            BirthYear = values[2] == @"\N" ? 0 : int.Parse(values[2]),
                            DeathYear = values[3] == @"\N" ? null : int.Parse(values[3]),
                            TitlePersons = new List<TitlePerson>(), // Initialize empty list
                            PersonProfessions = new List<PersonProfession>() // Initialize empty list
                        };

                        // Parse and add professions
                        var professions = values[4].Split(',');
                        foreach (var professionName in professions)
                        {
                            if (!string.IsNullOrEmpty(professionName))
                            {
                                // Check if the profession exists in the database
                                var profession = _dbContext.Professions.FirstOrDefault(p => p.ProfessionName == professionName);

                                // If the profession doesn't exist, create and add it to the database
                                if (profession == null)
                                {
                                    profession = new Profession { ProfessionName = professionName };
                                    _dbContext.Professions.Add(profession);
                                    _dbContext.SaveChanges(); // Save changes to generate the professionId
                                }

                                // Associate the profession with the person
                                person.PersonProfessions.Add(new PersonProfession { Nconst = person.Nconst, ProfessionId = profession.ProfessionId });
                            }
                        }

                        // Parse and add knownForTitles (TitlePersons)
                        var knownForTitles = values[5].Split(',');
                        foreach (var knownForTitle in knownForTitles)
                        {
                            if (!string.IsNullOrEmpty(knownForTitle))
                            {
                                person.TitlePersons.Add(new TitlePerson { Tconst = knownForTitle, Nconst = person.Nconst });
                            }
                        }

                        personsToAdd.Add(person); // Add person to the list
                        recordsProcessed++;

                        if (personsToAdd.Count >= 1000) // Insert in batches of 1000
                        {
                            _dbContext.Persons.AddRange(personsToAdd); // Bulk insert persons
                            _dbContext.SaveChanges(); // Save changes
                            personsToAdd.Clear(); // Clear the list
                        }
                    }

                    // Insert remaining persons
                    if (personsToAdd.Any())
                    {
                        _dbContext.Persons.AddRange(personsToAdd);
                        _dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during import of persons: \n {ex}");
            }
        }

        private void ImportTitleCrewFromFile(string filePath, int maxRecords)
        {
            try
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    // Skip header line
                    streamReader.ReadLine();

                    int recordsProcessed = 0;

                    while (!streamReader.EndOfStream && recordsProcessed < maxRecords)
                    {
                        var line = streamReader.ReadLine();
                        var values = line.Split('\t');

                        var titleId = values[0];
                        var directorIds = values[1].Split(',').Where(id => id != @"\N").ToList();
                        var writerIds = values[2].Split(',').Where(id => id != @"\N").ToList();

                        // Associate directors with titles
                        foreach (var directorId in directorIds)
                        {
                            if (_dbContext.Persons.Any(p => p.Nconst == directorId))
                            {
                                _dbContext.TitlePersons.Add(new TitlePerson { Tconst = titleId, Nconst = directorId });
                            }
                            else
                            {
                                Console.WriteLine($"Director with Nconst {directorId} not found in Persons table. TitlePerson not added.");
                            }
                        }

                        // Associate writers with titles
                        foreach (var writerId in writerIds)
                        {
                            if (_dbContext.Persons.Any(p => p.Nconst == writerId))
                            {
                                _dbContext.TitlePersons.Add(new TitlePerson { Tconst = titleId, Nconst = writerId });
                            }
                            else
                            {
                                Console.WriteLine($"Writer with Nconst {writerId} not found in Persons table. TitlePerson not added.");
                            }
                        }

                        recordsProcessed++;
                    }

                    _dbContext.SaveChanges(); // Save changes
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during import of title crew: \n {ex}");
            }
        }




    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using IMDBLib.DataBase;
using IMDBLib.Models;
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

        public void ImportData(string titleFilePath, string nameFilePath, string titleCrewFilePath, int batchSize = 1000)
        {
            using (var transactionScope = new TransactionScope())
            {
                try
                {
                    ImportTitlesFromFile(titleFilePath, batchSize);
                    ImportPersonsFromFile(nameFilePath, batchSize);
                    ImportTitleCrewFromFile(titleCrewFilePath, batchSize);

                    _dbContext.SaveChanges();

                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred during data import: {ex}");
                }
            }
        }

        private void ImportTitlesFromFile(string filePath, int batchSize)
        {
            try
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    streamReader.ReadLine(); // Skip header line

                    var titlesToAdd = new List<Title>();
                    int recordsProcessed = 0;

                    while (!streamReader.EndOfStream)
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
                            TitleGenres = new List<TitleGenre>(),
                            TitlePersons = new List<TitlePerson>()
                        };

                        var genres = values[8].Split(',');
                        foreach (var genre in genres)
                        {
                            if (!string.IsNullOrEmpty(genre))
                            {
                                int genreId;
                                var existingGenre = _dbContext.Genres.FirstOrDefault(g => g.GenreName == genre);
                                if (existingGenre != null)
                                {
                                    genreId = existingGenre.GenreId;
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

                        titlesToAdd.Add(title);
                        recordsProcessed++;

                        if (recordsProcessed % batchSize == 0)
                        {
                            _dbContext.Titles.AddRange(titlesToAdd);
                            _dbContext.SaveChanges();
                            titlesToAdd.Clear();
                        }
                    }

                    if (titlesToAdd.Any())
                    {
                        _dbContext.Titles.AddRange(titlesToAdd);
                        _dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during import of titles: {ex}");
            }
        }

        private void ImportPersonsFromFile(string filePath, int batchSize)
        {
            try
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    streamReader.ReadLine(); // Skip header line

                    var personsToAdd = new List<Person>();
                    int recordsProcessed = 0;

                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLine();
                        var values = line.Split('\t');

                        var person = new Person
                        {
                            Nconst = values[0],
                            PrimaryName = values[1],
                            BirthYear = values[2] == @"\N" ? 0 : int.Parse(values[2]),
                            DeathYear = values[3] == @"\N" ? null : int.Parse(values[3]),
                            TitlePersons = new List<TitlePerson>(),
                            PersonProfessions = new List<PersonProfession>()
                        };

                        var professions = values[4].Split(',');
                        foreach (var professionName in professions)
                        {
                            if (!string.IsNullOrEmpty(professionName))
                            {
                                var profession = _dbContext.Professions.FirstOrDefault(p => p.ProfessionName == professionName);
                                if (profession == null)
                                {
                                    profession = new Profession { ProfessionName = professionName };
                                    _dbContext.Professions.Add(profession);
                                    _dbContext.SaveChanges();
                                }

                                person.PersonProfessions.Add(new PersonProfession { Nconst = person.Nconst, ProfessionId = profession.ProfessionId });
                            }
                        }

                        var knownForTitles = values[5].Split(',');
                        foreach (var knownForTitle in knownForTitles)
                        {
                            if (!string.IsNullOrEmpty(knownForTitle))
                            {
                                person.TitlePersons.Add(new TitlePerson { Tconst = knownForTitle, Nconst = person.Nconst });
                            }
                        }

                        personsToAdd.Add(person);
                        recordsProcessed++;

                        if (recordsProcessed % batchSize == 0)
                        {
                            _dbContext.Persons.AddRange(personsToAdd);
                            _dbContext.SaveChanges();
                            personsToAdd.Clear();
                        }
                    }

                    if (personsToAdd.Any())
                    {
                        _dbContext.Persons.AddRange(personsToAdd);
                        _dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during import of persons: {ex}");
            }
        }

        private void ImportTitleCrewFromFile(string filePath, int batchSize)
        {
            try
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    streamReader.ReadLine(); // Skip header line

                    int recordsProcessed = 0;

                    while (!streamReader.EndOfStream)
                    {
                        var line = streamReader.ReadLine();
                        var values = line.Split('\t');

                        var titleId = values[0];
                        var directorIds = values[1].Split(',').Where(id => id != @"\N").ToList();
                        var writerIds = values[2].Split(',').Where(id => id != @"\N").ToList();

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

                        if (recordsProcessed % batchSize == 0)
                        {
                            _dbContext.SaveChanges();
                        }
                    }

                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during import of title crew: {ex}");
            }
        }
    }
}

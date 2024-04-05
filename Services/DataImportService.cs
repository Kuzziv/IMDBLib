using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Channels;
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

        public void ImportData(string titleFilePath, string nameFilePath, string titleCrewFilePath, int batchSize, int numberOfLines, int timeOutTime)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(timeOutTime)))
            {
                try
                {
                    CleanDatabase();

                    Console.WriteLine("Process has started");
                    ImportTitlesFromFile(titleFilePath, batchSize, numberOfLines);
                    ImportPersonsFromFile(nameFilePath, batchSize, numberOfLines);
                    ImportTitleCrewFromFile(titleCrewFilePath, batchSize, numberOfLines);

                    _dbContext.SaveChanges();

                    transactionScope.Complete();
                    Console.WriteLine("Process has completed successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred during data import: {ex}");
                }
            }
        }

        private void ImportTitlesFromFile(string filePath, int batchSize, int numberOfLines)
        {
            try
            {
                Console.WriteLine("Titles process has started");
                using (var streamReader = new StreamReader(filePath))
                {
                    streamReader.ReadLine(); // Skip header line

                    var titlesToAdd = new List<Title>();
                    int recordsProcessed = 0;
                    int totalLinesRead = 0;

                    while (!streamReader.EndOfStream && totalLinesRead < numberOfLines)
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
                            StartYear = ConvertStringToInt(values[5]) ?? 0,
                            EndYear = ConvertStringToInt(values[6]),
                            RuntimeMinutes = ConvertStringToInt(values[7]) ?? 0,
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
                        totalLinesRead++;

                        if (recordsProcessed % batchSize == 0)
                        {
                            Console.WriteLine("Titles has reached batch size. Saving changes. current line: " + totalLinesRead);
                            _dbContext.Titles.AddRange(titlesToAdd);
                            _dbContext.SaveChanges();
                            titlesToAdd.Clear();
                        }
                    }

                    if (titlesToAdd.Any())
                    {
                        _dbContext.Titles.AddRange(titlesToAdd);
                        _dbContext.SaveChanges();
                        Console.WriteLine("Titles process has successfully ended");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during import of titles: {ex}");
            }
        }

        private void ImportPersonsFromFile(string filePath, int batchSize, int numberOfLines)
        {
            try
            {
                Console.WriteLine("Persons process has started");
                using (var streamReader = new StreamReader(filePath))
                {
                    streamReader.ReadLine(); // Skip header line

                    var personsToAdd = new List<Person>();
                    int recordsProcessed = 0;
                    int totalLinesRead = 0;

                    while (!streamReader.EndOfStream && totalLinesRead < numberOfLines)
                    {
                        var line = streamReader.ReadLine();
                        var values = line.Split('\t');

                        var person = new Person
                        {
                            Nconst = values[0],
                            PrimaryName = values[1],
                            BirthYear = ConvertStringToInt(values[2]) ?? 0,
                            DeathYear = ConvertStringToInt(values[3]),
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

                        personsToAdd.Add(person);
                        recordsProcessed++;
                        totalLinesRead++;

                        if (recordsProcessed % batchSize == 0)
                        {
                            Console.WriteLine("Persons has reached batch size. Saving changes. current line: " + totalLinesRead);
                            _dbContext.Persons.AddRange(personsToAdd);
                            _dbContext.SaveChanges();
                            personsToAdd.Clear();
                        }
                    }

                    if (personsToAdd.Any())
                    {
                        _dbContext.Persons.AddRange(personsToAdd);
                        _dbContext.SaveChanges();
                        Console.WriteLine("Persons process has successfully ended");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during import of persons: {ex}");
            }
        }

        private void ImportTitleCrewFromFile(string filePath, int batchSize, int numberOfLines)
        {
            try
            {
                Console.WriteLine("TitleCrew process has started");
                using (var streamReader = new StreamReader(filePath))
                {
                    streamReader.ReadLine(); // Skip header line

                    int recordsProcessed = 0;
                    int totalLinesRead = 0;

                    while (!streamReader.EndOfStream && totalLinesRead < numberOfLines)
                    {
                        var line = streamReader.ReadLine();
                        var values = line.Split('\t');

                        var titleId = values[0];
                        var directorIds = ConvertStringToIntList(values[1]);
                        var writerIds = ConvertStringToIntList(values[2]);

                        if (directorIds != null)
                        {
                            foreach (var directorId in directorIds)
                            {
                                if (directorId.HasValue && _dbContext.Persons.Any(p => p.Nconst == directorId.Value.ToString()))
                                {
                                    _dbContext.TitlePersons.Add(new TitlePerson { Tconst = titleId, Nconst = directorId.Value.ToString() });
                                }                                
                            }
                        }
                        
                        if (writerIds != null)
                        { 
                            foreach (var writerId in writerIds)
                            {
                                if (writerId.HasValue && _dbContext.Persons.Any(p => p.Nconst == writerId.Value.ToString()))
                                {
                                    _dbContext.TitlePersons.Add(new TitlePerson { Tconst = titleId, Nconst = writerId.Value.ToString() });
                                }                                
                            }
                        }

                        recordsProcessed++;
                        totalLinesRead++;

                        if (recordsProcessed % batchSize == 0)
                        {
                            Console.WriteLine("TitleCrew has reached batch size. Saving changes. current line: " + totalLinesRead);
                            _dbContext.SaveChanges();
                        }
                    }

                    _dbContext.SaveChanges();
                    Console.WriteLine("TitleCrew process has successfully ended");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during import of title crew: {ex}");
            }
        }

        private int? ConvertStringToInt(string value)
        {
            if (value == @"\N")
            {
                return null;
            }
            else
            {
                if (int.TryParse(value, out int result))
                {
                    return result;
                }
                else
                {
                    // Handle parsing failure, maybe log an error or throw an exception
                    return null; // or throw an exception if you prefer
                }
            }
        }

        private List<int?> ConvertStringToIntList(string value)
        {
            if (value == @"\N")
            {
                return null;
            }
            else
            {
                var ids = value.Split(',').Select(id =>
                {
                    if (int.TryParse(id, out int result))
                    {
                        return (int?)result;
                    }
                    else
                    {
                        // Handle parsing failure, maybe log an error or throw an exception
                        return null; // or throw an exception if you prefer
                    }
                }).ToList();

                return ids;
            }
        }

        public void CleanDatabase()
        {
            try
            {
                Console.WriteLine("Cleaning database...");
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM TitlePersons");
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM Titles");
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM Persons");
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM Professions");
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM PersonProfessions");
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM TitleGenres");
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM Genres");
                Console.WriteLine("Database cleaned successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during cleaning of database: {ex}");
            }
        }

    }
}

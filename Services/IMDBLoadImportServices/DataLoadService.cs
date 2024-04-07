using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMDBLib.DataBase;
using IMDBLib.Models.Movie;
using IMDBLib.Models.People;
using IMDBLib.Models.Records;
using Microsoft.EntityFrameworkCore;

namespace IMDBLib.Services
{
    public class DataLoadService
    {
        private readonly IMDBDbContext _dbContext;
        private readonly DataImportService _dataImportService;
        private readonly DataDeleteService _dataDeleteService;

        public DataLoadService(IMDBDbContext dbContext, DataImportService dataImportService, DataDeleteService dataDeleteService)
        {
            _dbContext = dbContext;
            _dataImportService = dataImportService;
            _dataDeleteService = dataDeleteService;
        }

        public async Task LoadDataAsync(string titleBasicsPath, string nameBasicsPath, string crewBasicsPath, int batchSize, int numLines)
        {
            try
            {
                await _dataDeleteService.DeleteAllDataAsync();
                var (titleRecords, nameRecords, crewRecords) = await _dataImportService.DataImportAsync(titleBasicsPath, nameBasicsPath, crewBasicsPath, batchSize, numLines);

                await Console.Out.WriteLineAsync("Load of entity has started");

                // Calculate total number of batches
                int totalBatches = (int)Math.Ceiling((double)titleRecords.Count / batchSize) +
                           (int)Math.Ceiling((double)nameRecords.Count / batchSize) +
                           (int)Math.Ceiling((double)crewRecords.Count / batchSize);
                int currentBatch = 0;

                await LinkAndSaveDataAsync(titleRecords, nameRecords, crewRecords, batchSize, () =>
                {
                    currentBatch++;
                    Console.WriteLine($"Batch {currentBatch} of {totalBatches} completed");
                });

                await Console.Out.WriteLineAsync("Load of entity has completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during data loading: {ex}");
            }
        }


        private async Task LinkAndSaveDataAsync(Dictionary<string, TitleBasicsRecord> titleRecords,
            Dictionary<string, NameBasicsRecord> nameRecords,
            Dictionary<string, CrewBasicsRecord> crewRecords,
            int batchSize,
            Action batchCompletedCallback)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var genres = new List<Genre>();
                var titleGenres = new List<TitleGenre>();
                var titles = new List<Title>();
                var persons = new List<Person>();
                var professions = new List<Profession>();
                var titlePersons = new List<TitlePerson>();
                var personProfessions = new List<PersonProfession>();

                foreach (var titleBatch in titleRecords.Values.Batch(batchSize))
                {
                    foreach (var titleRecord in titleBatch)
                    {
                        var existingTitle = titles.FirstOrDefault(t => t.Tconst == titleRecord.tconst);
                        var title = existingTitle ?? new Title
                        {
                            Tconst = titleRecord.tconst,
                            TitleType = titleRecord.titleType,
                            PrimaryTitle = titleRecord.primaryTitle,
                            OriginalTitle = titleRecord.originalTitle,
                            IsAdult = titleRecord.isAdult,
                            StartYear = TryParseInt(titleRecord.startYear),
                            EndYear = TryParseNullableInt(titleRecord.endYear),
                            RuntimeMinutes = TryParseInt(titleRecord.runtimeMinutes)
                        };

                        // Link genres to title
                        var genreNames = titleRecord.genres.Split(',');
                        foreach (var genreName in genreNames)
                        {
                            if (!string.IsNullOrWhiteSpace(genreName))
                            {
                                var existingGenre = genres.FirstOrDefault(g => g.GenreName == genreName);
                                if (existingGenre != null)
                                {
                                    // Use the existing genre
                                    titleGenres.Add(new TitleGenre { Genre = existingGenre, Title = title });
                                }
                                else
                                {
                                    var genre = new Genre { GenreName = genreName };
                                    genres.Add(genre);
                                    titleGenres.Add(new TitleGenre { Genre = genre, Title = title });
                                }
                            }
                        }

                        titles.Add(title);
                    }

                    // Report batch completion
                    batchCompletedCallback?.Invoke();
                }

                foreach (var nameBatch in nameRecords.Values.Batch(batchSize))
                {
                    foreach (var nameRecord in nameBatch)
                    {
                        var existingPerson = persons.FirstOrDefault(p => p.Nconst == nameRecord.nconst);
                        var person = existingPerson ?? new Person
                        {
                            Nconst = nameRecord.nconst,
                            PrimaryName = nameRecord.primaryName,
                            BirthYear = TryParseInt(nameRecord.birthYear),
                            DeathYear = TryParseNullableInt(nameRecord.deathYear)
                        };

                        // Link professions to person
                        var professionNames = nameRecord.primaryProfession.Split(',');
                        foreach (var professionName in professionNames)
                        {
                            if (!string.IsNullOrWhiteSpace(professionName))
                            {
                                var existingProfession = professions.FirstOrDefault(p => p.ProfessionName == professionName);
                                if (existingProfession != null)
                                {
                                    // Use the existing profession
                                    personProfessions.Add(new PersonProfession { Profession = existingProfession, Person = person });
                                }
                                else
                                {
                                    var profession = new Profession { ProfessionName = professionName };
                                    professions.Add(profession);
                                    personProfessions.Add(new PersonProfession { Profession = profession, Person = person });
                                }
                            }
                        }

                        persons.Add(person);
                    }

                    // Report batch completion
                    batchCompletedCallback?.Invoke();
                }

                foreach (var crewBatch in crewRecords.Values.Batch(batchSize))
                {
                    foreach (var crewRecord in crewBatch)
                    {
                        var title = titles.FirstOrDefault(t => t.Tconst == crewRecord.tconst);
                        if (title != null)
                        {
                            var directorIds = crewRecord.directors.Split(',');
                            var writerIds = crewRecord.writers.Split(',');

                            foreach (var personId in directorIds.Union(writerIds))
                            {
                                if (!string.IsNullOrWhiteSpace(personId) && nameRecords.TryGetValue(personId, out NameBasicsRecord nameRecord))
                                {
                                    var existingPerson = persons.FirstOrDefault(p => p.Nconst == nameRecord.nconst);
                                    var person = existingPerson ?? new Person
                                    {
                                        Nconst = nameRecord.nconst,
                                        PrimaryName = nameRecord.primaryName,
                                        BirthYear = TryParseInt(nameRecord.birthYear),
                                        DeathYear = TryParseNullableInt(nameRecord.deathYear)
                                    };

                                    persons.Add(person);
                                    var existingTitlePerson = titlePersons.FirstOrDefault(tp => tp.Person.Nconst == person.Nconst && tp.Title.Tconst == title.Tconst);
                                    if (existingTitlePerson == null)
                                    {
                                        titlePersons.Add(new TitlePerson { Person = person, Title = title });
                                    }
                                }
                            }
                        }
                    }

                    // Report batch completion
                    batchCompletedCallback?.Invoke();
                }
                // Log the start of data saving
                await Console.Out.WriteLineAsync("Saving to database process has started...");

                // Log the start of each data saving step
                await Console.Out.WriteLineAsync("Starting to save genres...");
                await _dbContext.Genres.AddRangeAsync(genres);
                await Console.Out.WriteLineAsync("Genres save completed.");

                await Console.Out.WriteLineAsync("Starting to save titles...");
                await _dbContext.Titles.AddRangeAsync(titles);
                await Console.Out.WriteLineAsync("Titles save completed.");

                await Console.Out.WriteLineAsync("Starting to save title genres...");
                await _dbContext.TitleGenres.AddRangeAsync(titleGenres);
                await Console.Out.WriteLineAsync("Title genres save completed.");

                await Console.Out.WriteLineAsync("Starting to save persons...");
                await _dbContext.Persons.AddRangeAsync(persons);
                await Console.Out.WriteLineAsync("Persons save completed.");

                await Console.Out.WriteLineAsync("Starting to save professions...");
                await _dbContext.Professions.AddRangeAsync(professions);
                await Console.Out.WriteLineAsync("Professions save completed.");

                await Console.Out.WriteLineAsync("Starting to save person professions...");
                await _dbContext.PersonProfessions.AddRangeAsync(personProfessions);
                await Console.Out.WriteLineAsync("Person professions save completed.");

                await Console.Out.WriteLineAsync("Starting to save title persons...");
                await _dbContext.TitlePersons.AddRangeAsync(titlePersons);
                await Console.Out.WriteLineAsync("Title persons save completed.");

                // Save changes to the database
                await _dbContext.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();
                Console.WriteLine("Database changes saved successfully");

            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an exception
                await transaction.RollbackAsync();
                Console.WriteLine($"Error occurred during data loading: {ex}");
            }
        }


        private int TryParseInt(string value)
        {
            return int.TryParse(value, out int result) ? result : 0;
        }

        private int? TryParseNullableInt(string value)
        {
            return int.TryParse(value, out int result) ? result : (int?)null;
        }
    }
}

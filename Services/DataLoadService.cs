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
                foreach (var titleBatch in titleRecords.Values.Batch(batchSize))
                {
                    var genres = new List<Genre>();
                    var titleGenres = new List<TitleGenre>();

                    foreach (var titleRecord in titleBatch)
                    {
                        var existingTitle = await _dbContext.Titles.FindAsync(titleRecord.tconst);
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
                                    _dbContext.Genres.Add(genre);
                                    genres.Add(genre);
                                    titleGenres.Add(new TitleGenre { Genre = genre, Title = title });
                                }
                            }
                        }

                        if (existingTitle == null)
                            _dbContext.Titles.Add(title);
                    }

                    await _dbContext.Genres.AddRangeAsync(genres);
                    await _dbContext.TitleGenres.AddRangeAsync(titleGenres);

                    // Report batch completion
                    batchCompletedCallback?.Invoke();
                }

                foreach (var nameBatch in nameRecords.Values.Batch(batchSize))
                {
                    var professions = new List<Profession>();
                    var personProfessions = new List<PersonProfession>();

                    foreach (var nameRecord in nameBatch)
                    {
                        // Check if the person already exists in the context
                        var existingPerson = await _dbContext.Persons.FirstOrDefaultAsync(p => p.Nconst == nameRecord.nconst);
                        Person person;

                        if (existingPerson != null)
                        {
                            // Use the existing person
                            person = existingPerson;
                        }
                        else
                        {
                            // Create a new person
                            person = new Person
                            {
                                Nconst = nameRecord.nconst,
                                PrimaryName = nameRecord.primaryName,
                                BirthYear = TryParseInt(nameRecord.birthYear),
                                DeathYear = TryParseNullableInt(nameRecord.deathYear)
                            };

                            _dbContext.Persons.Add(person);
                        }

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
                                    _dbContext.Professions.Add(profession);
                                    professions.Add(profession);
                                    personProfessions.Add(new PersonProfession { Profession = profession, Person = person });
                                }
                            }
                        }

                    }

                    await _dbContext.Professions.AddRangeAsync(professions);
                    await _dbContext.PersonProfessions.AddRangeAsync(personProfessions);

                    // Report batch completion
                    batchCompletedCallback?.Invoke();
                }

                foreach (var crewBatch in crewRecords.Values.Batch(batchSize))
                {
                    foreach (var crewRecord in crewBatch)
                    {
                        var title = await _dbContext.Titles.FirstOrDefaultAsync(t => t.Tconst == crewRecord.tconst);
                        if (title != null)
                        {
                            var directorIds = crewRecord.directors.Split(',');
                            var writerIds = crewRecord.writers.Split(',');

                            foreach (var personId in directorIds.Union(writerIds))
                            {
                                if (!string.IsNullOrWhiteSpace(personId) && nameRecords.TryGetValue(personId, out NameBasicsRecord nameRecord))
                                {
                                    var existingPerson = await _dbContext.Persons.FirstOrDefaultAsync(p => p.Nconst == nameRecord.nconst);
                                    Person person;

                                    if (existingPerson != null)
                                    {
                                        // Use the existing person
                                        person = existingPerson;
                                    }
                                    else
                                    {
                                        // Create a new person
                                        person = new Person
                                        {
                                            Nconst = nameRecord.nconst,
                                            PrimaryName = nameRecord.primaryName,
                                            BirthYear = TryParseInt(nameRecord.birthYear),
                                            DeathYear = TryParseNullableInt(nameRecord.deathYear)
                                        };

                                        _dbContext.Persons.Add(person);
                                    }

                                    // Add the person and link to the title
                                    _dbContext.TitlePersons.Add(new TitlePerson { Person = person, Title = title });
                                }
                            }
                        }
                    }

                    await _dbContext.SaveChangesAsync(); // Add the task to the list

                    // Report batch completion
                    batchCompletedCallback?.Invoke();
                }

                await transaction.CommitAsync(); // Commit the transaction
                Console.WriteLine("Database changes saved successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback the transaction in case of an exception
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

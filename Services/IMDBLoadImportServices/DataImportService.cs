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
using IMDBLib.Models.Records;
using Microsoft.EntityFrameworkCore;

namespace IMDBLib.Services
{
    public class DataImportService
    {
        public async Task<(Dictionary<string, TitleBasicsRecord>, Dictionary<string, NameBasicsRecord>, Dictionary<string, CrewBasicsRecord>)> DataImportAsync(string titleFilePath, string nameFilePath, string crewFilePath, int batchSize, int numLines)
        {
            Console.WriteLine("Import data has started");

            var titleTask = LoadTitleBasicsDataAsync(titleFilePath, batchSize, numLines);
            var nameTask = LoadNameBasicsDataAsync(nameFilePath, batchSize, numLines);
            var crewTask = LoadCrewBasicsDataAsync(crewFilePath, batchSize, numLines);

            var titleRecords = await titleTask;
            var nameRecords = await nameTask;
            var crewRecords = await crewTask;

            Console.WriteLine("Load data has completed");

            return (titleRecords, nameRecords, crewRecords);
        }

        private async Task<Dictionary<string, TitleBasicsRecord>> LoadTitleBasicsDataAsync(string filePath, int batchSize, int numLines)
        {
            var records = new Dictionary<string, TitleBasicsRecord>();

            using (var reader = new StreamReader(filePath))
            {
                int linesProcessed = 0;
                while (!reader.EndOfStream && linesProcessed < numLines)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split('\t');

                    bool.TryParse(values[4], out bool isAdult);

                    records.Add(values[0], new TitleBasicsRecord
                    {
                        tconst = values[0],
                        titleType = values[1],
                        primaryTitle = values[2],
                        originalTitle = values[3],
                        isAdult = isAdult,
                        startYear = values[5],
                        endYear = values[6],
                        runtimeMinutes = values[7],
                        genres = values[8]
                    });

                    linesProcessed++;

                    if (linesProcessed % (batchSize * 10) == 0 || linesProcessed == numLines)
                    {
                        Console.WriteLine($"Processed {linesProcessed} Title records");
                    }
                }
            }

            return records;
        }

        private async Task<Dictionary<string, NameBasicsRecord>> LoadNameBasicsDataAsync(string filePath, int batchSize, int numLines)
        {
            var records = new Dictionary<string, NameBasicsRecord>();

            using (var reader = new StreamReader(filePath))
            {
                int linesProcessed = 0;
                while (!reader.EndOfStream && linesProcessed < numLines)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split('\t');

                    records.Add(values[0], new NameBasicsRecord
                    {
                        nconst = values[0],
                        primaryName = values[1],
                        birthYear = values[2],
                        deathYear = values[3],
                        primaryProfession = values[4],
                        knownForTitles = values[5]
                    });

                    linesProcessed++;

                    if (linesProcessed % (batchSize * 10) == 0 || linesProcessed == numLines)
                    {
                        Console.WriteLine($"Processed {linesProcessed} Name records");
                    }
                }
            }

            return records;
        }

        private async Task<Dictionary<string, CrewBasicsRecord>> LoadCrewBasicsDataAsync(string filePath, int batchSize, int numLines)
        {
            var records = new Dictionary<string, CrewBasicsRecord>();

            using (var reader = new StreamReader(filePath))
            {
                int linesProcessed = 0;
                while (!reader.EndOfStream && linesProcessed < numLines)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split('\t');

                    records.Add(values[0], new CrewBasicsRecord
                    {
                        tconst = values[0],
                        directors = values[1],
                        writers = values[2]
                    });

                    linesProcessed++;

                    if (linesProcessed % (batchSize * 10) == 0 || linesProcessed == numLines)
                    {
                        Console.WriteLine($"Processed {linesProcessed} Crew records");
                    }
                }
            }

            return records;
        }

    }
}

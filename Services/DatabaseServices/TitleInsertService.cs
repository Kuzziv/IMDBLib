using IMDBLib.Models.Movie;
using IMDBLib.Models.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Services.DatabaseServices
{
    public class TitleInsertService
    {
        public static (List<Title>, List<TitleType>, List<Genre>, List<Title_Genre>) TitleBasicsProcessor(List<TitleBasicsRecord> titleRecords)
        {
            int count = 0;
            var titles = new List<Title>();
            var titleTypes = new List<TitleType>();
            var genres = new List<Genre>();
            var title_Genres = new List<Title_Genre>();

            foreach (var record in titleRecords)
            {
                // Create or get TitleType
                var titleType = titleTypes.FirstOrDefault(t => t.Type == record.titleType);
                if (titleType == null)
                {
                    titleType = new TitleType { Type = record.titleType };
                    titleTypes.Add(titleType);
                }


                // Create Title
                var title = new Title
                {
                    Tconst = record.tconst,
                    PrimaryTitle = record.primaryTitle,
                    OriginalTitle = record.originalTitle,
                    IsAdult = record.isAdult,
                    StartYear = ParseYear(record.startYear) ?? 0,
                    EndYear = ParseYear(record.endYear) ?? 0,
                    RuntimeMinutes = ParseInt(record.runtimeMinutes),

                };

                // Associate TitleType with Title
                title.TitleType = titleType;

                // Assign the TitleTypeId
                title.TitleTypeId = titleType.Id;

                titles.Add(title);

                // Create or get Genre
                var genreTypes = record.genres.Split(',');
                foreach (var genreType in genreTypes)
                {
                    var genre = genres.FirstOrDefault(g => g.GenreName == genreType);
                    if (genre == null)
                    {
                        genre = new Genre { GenreName = genreType };
                        genres.Add(genre);
                    }

                    // Check if the Title_Genre association already exists
                    if (!title_Genres.Any(tg => tg.TitleTconst == record.tconst && tg.GenreId == genre.Id))
                    {
                        // Create Title_Genre
                        var titleGenre = new Title_Genre { TitleTconst = record.tconst, GenreId = genre.Id };
                        title_Genres.Add(titleGenre);
                    }
                }

                // Increment the count
                count++;

                // log every 1000 records
                if (count % 1000 == 0)
                {
                    Console.WriteLine($"Processed {count} title records");
                }

            }

            return (titles, titleTypes, genres, title_Genres);
        }

        private static int? ParseYear(string yearString)
        {
            if (yearString == @"\N" || !int.TryParse(yearString, out int year))
            {
                return null; // Return null if yearString is "\N" or cannot be parsed
            }
            else
            {
                return year;
            }
        }

        private static int ParseInt(string intString)
        {
            return int.TryParse(intString, out int result) ? result : 0;
        }
    }
}


using IMDBLib.Models.Movie;
using IMDBLib.Models.People;
using IMDBLib.Models.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Services.DatabaseServices
{
    public class CrewInsertService
    {
        public static (List<Title_Crew>, List<Writers>, List<Directors>) CrewProcessRecords(List<CrewBasicsRecord> records, List<Title> titles, List<Crew> crews)
        {
            Console.WriteLine("Processing Crew Records...");

            int count = 0;
            int recordsNotFound = 0;

            // Create a dictionary to store Title_Crew objects by TitleTconst
            Dictionary<string, Title_Crew> titleCrewDictionary = new Dictionary<string, Title_Crew>();

            // Lists to hold Writers and Directors
            List<Writers> writers = new List<Writers>();
            List<Directors> directors = new List<Directors>();

            foreach (var record in records)
            {
                // First check if the tconst exists in the titles list
                var title = titles.FirstOrDefault(t => t.Tconst == record.tconst);
                if (title == null)
                {
                    recordsNotFound++;
                    continue;
                }

                // Check if Title_Crew for this TitleTconst already exists
                if (!titleCrewDictionary.TryGetValue(title.Tconst, out Title_Crew titleCrew))
                {
                    // If not, create a new Title_Crew
                    titleCrew = new Title_Crew { TitleTconst = title.Tconst };
                    titleCrewDictionary.Add(title.Tconst, titleCrew);
                }

                // Process directors
                var directorsList = record.directors.Split(',');
                foreach (var director in directorsList)
                {
                    var crew = crews.FirstOrDefault(c => c.Nconst == director);
                    if (crew != null)
                    {
                        directors.Add(new Directors { DirectorNconst = director, Title_CrewId = titleCrew.Id });
                    }
                }

                // Process writers
                var writersList = record.writers.Split(',');
                foreach (var writer in writersList)
                {
                    var crew = crews.FirstOrDefault(c => c.Nconst == writer);
                    if (crew != null)
                    {
                        writers.Add(new Writers { WriterNconst = writer, Title_CrewId = titleCrew.Id });
                    }
                }

                count++;

                // Add a line to see the progress for every 1000 crews
                if (count % 1000 == 0)
                {
                    Console.WriteLine($"Processed {count} crews, records not found {recordsNotFound}");
                }
            }

            return (titleCrewDictionary.Values.ToList(), writers, directors);
        }
    }
}

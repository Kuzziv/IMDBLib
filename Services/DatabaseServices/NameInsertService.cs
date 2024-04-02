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
    public class NameInsertService
    {
        public static (List<Crew>, List<Crew_Profession>, List<Job>, List<Known_For_Titles>) NameProcessRecords(List<NameBasicsRecord> nameBasicsRecords, List<Title> titles)
        {
            Console.WriteLine("Processing Name Records...");

            int count = 0;
            int recordsNotFound = 0;

            // Create a list to hold the Crew records
            List<Crew> crews = new List<Crew>();
            List<Crew_Profession> crew_Professions = new List<Crew_Profession>();
            List<Job> jobs = new List<Job>();
            List<Known_For_Titles> known_For_Titles = new List<Known_For_Titles>();

            foreach (var record in nameBasicsRecords)
            {
                // firt look up if the tconst exists in the titles list
                var knownForTitles = record.knownForTitles.Split(',');
                foreach (var knownForTitle in knownForTitles)
                {
                    var title = titles.FirstOrDefault(t => t.Tconst == knownForTitle);
                    if (title == null)
                    {
                        recordsNotFound++;
                        continue;
                    }
                }

                // Create or get job and associate with crew
                var primaryProfessions = record.primaryProfession.Split(',');
                foreach (var profess in primaryProfessions)
                {
                    // look up if the profession exists in the job list and get the job, if not then create the new job
                    var job = jobs.FirstOrDefault(j => j.JobName == profess);
                    if (job == null)
                    {
                        // check for whitespace or empty string
                        if (string.IsNullOrWhiteSpace(profess))
                        {
                            continue;
                        }
                        job = new Job { JobName = profess };
                        jobs.Add(job);
                    }

                    // Create or get crew_profession and associate with crew and job
                    var crewProfession = crew_Professions.FirstOrDefault(cp => cp.CrewNconst == record.nconst && cp.JobId == job.Id);
                    if (crewProfession == null)
                    {
                        crewProfession = new Crew_Profession { CrewNconst = record.nconst, JobId = job.Id };
                        crew_Professions.Add(crewProfession);
                    }

                    // Create or get crew and associate with crew_profession
                    var crew = crews.FirstOrDefault(c => c.Nconst == record.nconst);
                    if (crew == null)
                    {
                        crew = new Crew { Nconst = record.nconst, PrimaryName = record.primaryName, BirthYear = record.birthYear, DeathYear = record.deathYear };
                        crews.Add(crew);
                    }
                }

                // Create or get known_for_titles and associate with crew
                foreach (var knownForTitle in knownForTitles)
                {
                    var title = titles.FirstOrDefault(t => t.Tconst == knownForTitle);
                    if (title == null)
                    {
                        recordsNotFound++;
                        continue;
                    }
                    var knownForTitleRecord = known_For_Titles.FirstOrDefault(kft => kft.CrewNconst == record.nconst && kft.TitleTconst == title.Tconst);
                    if (knownForTitleRecord == null)
                    {
                        knownForTitleRecord = new Known_For_Titles { CrewNconst = record.nconst, TitleTconst = title.Tconst };
                        known_For_Titles.Add(knownForTitleRecord);
                    }
                }

                count++;

                // Add a line to see the progress for every 100 crews
                if (count % 1000 == 0)
                {
                    Console.WriteLine($"Processed {count} Names, records not found {recordsNotFound}");
                }
            }

            Console.WriteLine("Crew processing has finished");

            return (crews, crew_Professions, jobs, known_For_Titles);
        }

    }
}

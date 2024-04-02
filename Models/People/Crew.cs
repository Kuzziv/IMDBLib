using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.People
{
    public class Crew
    {
        [Key]
        public string Nconst { get; set; }

        public string PrimaryName { get; set; }

        public string BirthYear { get; set; }

        public string DeathYear { get; set; }

        // Navigation property
        public ICollection<Writers> Writers { get; set; }

        // Navigation property
        public ICollection<Directors> Directors { get; set; }

        // Navigation property
        public ICollection<Crew_Profession> Crew_Professions { get; set; }

        // Navigation property
        public ICollection<Known_For_Titles> Know_For_Titles { get; set; }

    }
}

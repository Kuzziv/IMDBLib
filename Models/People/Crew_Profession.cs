using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.People
{
    public class Crew_Profession
    {
        public static int _lastId = 0;

        [Key]
        public int Id { get; set; }

        [ForeignKey("Crew")] // Specify the foreign key relationship
        public string CrewNconst { get; set; } // Change the data type to match the primary key type in Crew
        public Crew Crew { get; set; }

        [ForeignKey("Job")] // Specify the foreign key relationship
        public int JobId { get; set; }
        public Job Job { get; set; }

        public Crew_Profession()
        {
            // Increment the static variable and assign it to the Id property
            Id = ++_lastId;
        }
    }
}

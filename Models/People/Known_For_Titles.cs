using IMDBLib.Models.Movie;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.People
{
    public class Known_For_Titles
    {
        public static int _lastId = 0;

        [Key]
        public int Id { get; set; }

        [ForeignKey("Crew")]
        public string CrewNconst { get; set; }
        public Crew Crew { get; set; }

        [ForeignKey("Title")]
        public string TitleTconst { get; set; }
        public Title Title { get; set; }

        public Known_For_Titles()
        {
            // Increment the static variable and assign it to the Id property
            Id = ++_lastId;
        }
    }
}

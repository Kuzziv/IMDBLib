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
    public class Title_Crew
    {
        public static int _lastId = 0;


        [Key]
        public int Id { get; set; }

        [ForeignKey("Title")]
        public string TitleTconst { get; set; }
        public Title Title { get; set; }

        public Title_Crew()
        {
            // Increment the static variable and assign it to the Id property
            Id = ++_lastId;
        }

        // Navigation property to Writers
        public ICollection<Writers> Writers { get; set; }

        // Navigation property to Directors
        public ICollection<Directors> Directors { get; set; }



    }
}

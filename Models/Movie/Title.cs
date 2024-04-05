using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMDBLib.Models.People;

namespace IMDBLib.Models.Movie
{
    public class Title
    {
        [Key]
        [Required]
        public string Tconst { get; set; }

        [ForeignKey("TitleType")]
        public int TitleTypeId { get; set; }
        public TitleType TitleType { get; set; }

        [Required]
        public string PrimaryTitle { get; set; }
        public string OriginalTitle { get; set; }

        [Required]
        public bool IsAdult { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public int RuntimeMinutes { get; set; }

        // Define the navigation property to TitleGenre
        public ICollection<Title_Genre> Title_Genres { get; set; }

        // Define the navigation property to TitleCrew
        public Title_Crew Title_Crew { get; set; }

        // Define the navigation property to Know_For_Titles
        public ICollection<Known_For_Titles> Know_For_Titles { get; set; }
    }
}

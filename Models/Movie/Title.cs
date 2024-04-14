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
        public string Tconst { get; set; }

        public string TitleType { get; set; }
        public string PrimaryTitle { get; set; }
        public string OriginalTitle { get; set; }
        public bool IsAdult { get; set; }
        public int StartYear { get; set; }
        public int? EndYear { get; set; }
        public int RuntimeMinutes { get; set; }

        public ICollection<TitleGenre> TitleGenres { get; set; }
        public ICollection<TitlePerson> TitlePersons { get; set; }
    }
}

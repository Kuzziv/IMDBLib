using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.Movie
{
    public class TitleGenre
    {
        public string Tconst { get; set; }
        public int GenreId { get; set; }
        public Title Title { get; set; }
        public Genre Genre { get; set; }
    }
}

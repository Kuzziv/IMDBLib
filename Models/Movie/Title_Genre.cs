using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.Movie
{
    public class Title_Genre
    {
        private static int _lastId = 0;

        [Key]
        public int Id { get; set; }

        [ForeignKey("Title")]
        public string TitleTconst { get; set; }
        public Title Title { get; set; }

        [ForeignKey("Genre")]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public Title_Genre()
        {
            // Increment the static variable and assign it to the Id property
            //Id = ++_lastId;
        }
    }
}

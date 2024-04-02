using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.Movie
{
    public class Genre
    {
        private static int _lastId = 0;

        [Key]
        public int Id { get; set; }

        [Required]
        public string GenreName { get; set; }

        public Genre()
        {
            // Increment the static variable and assign it to the Id property
            Id = ++_lastId;
        }

        // Define the navigation property to TitleGenre
        public ICollection<Title_Genre> Title_Genres { get; set; }

        // Define the navigation property to Title
        public ICollection<Title> Titles { get; set; }

    }
}

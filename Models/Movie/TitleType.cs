using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.Movie
{
    public class TitleType
    {
        private static int _lastId = 0;

        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }

        public TitleType()
        {
            // Increment the static variable and assign it to the Id property
            Id = ++_lastId;
        }

        public ICollection<Title> Titles { get; set; }
    }
}

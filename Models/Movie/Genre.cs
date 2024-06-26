﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.Movie
{
    public class Genre
    {
        public int GenreId { get; set; }

        public string GenreName { get; set; }

        public ICollection<TitleGenre> TitleGenres { get; set; }
    }
}

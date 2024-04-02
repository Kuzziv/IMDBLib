using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.Views
{
    public class MovieView
    {
        public long ID { get; set; }  // Assuming ID is an integer
        public string Tconst { get; set; }
        public string PrimaryTitle { get; set; }
        public string OriginalTitle { get; set; }
        public bool IsAdult { get; set; }
        public int StartYear { get; set; }  // Assuming StartYear is an integer
        public int? EndYear { get; set; }   // Assuming EndYear is an integer and nullable
        public int RuntimeMinutes { get; set; }  // Assuming RuntimeMinutes is an integer and nullable
        public string Type { get; set; }
        public string GenreNames { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.Views
{
    public class MovieView
    {
        public int ID { get; set; }  // Assuming ID is an integer
        public string Tconst { get; set; }
        public string PrimaryTitle { get; set; }
        public string OriginalTitle { get; set; }
        public bool IsAdult { get; set; }
        public long StartYear { get; set; }  // Assuming StartYear is an integer
        public long? EndYear { get; set; }   // Assuming EndYear is an integer and nullable
        public long? RuntimeMinutes { get; set; }  // Assuming RuntimeMinutes is an integer and nullable
        public string Type { get; set; }
        public string GenreNames { get; set; }
    }
}

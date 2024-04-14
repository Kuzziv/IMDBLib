using IMDBLib.Models.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.People
{
    public class TitlePerson
    {
        public string Tconst { get; set; }
        public string Nconst { get; set; }
        public Title Title { get; set; }
        public Person Person { get; set; }
    }
}

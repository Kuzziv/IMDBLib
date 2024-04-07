using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.Views
{
    public class PersonView
    {
        public string Nconst { get; set; }
        public string PrimaryName { get; set; }
        public int BirthYear { get; set; }
        public int? DeathYear { get; set; }
        public string ProfessionNames { get; set; }
    }
}

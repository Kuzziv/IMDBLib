using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.Views
{
    public class PersonView
    {
        public long Id { get; set; }
        public string Nconst { get; set; }
        public string PrimaryName { get; set; }
        public string BirthYear { get; set; }
        public string DeathYear { get; set; }
        public string JobNames { get; set; }
    }
}

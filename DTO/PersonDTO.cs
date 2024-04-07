using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.DTO
{
    public class PersonDTO
    {
        public string Nconst { get; set; }
        public string PrimaryName { get; set; }
        public int BirthYear { get; set; }
        public int? DeathYear { get; set; }
        public List<string> ProfessionNames { get; set; }
    }
}

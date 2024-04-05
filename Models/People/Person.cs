using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.People
{
    public class Person
    {
        public string Nconst { get; set; }

        public string PrimaryName { get; set; }
        public int BirthYear { get; set; }
        public int? DeathYear { get; set; }

        public ICollection<TitlePerson> TitlePersons { get; set; }
        public ICollection<PersonProfession> PersonProfessions { get; set; }
    }
}

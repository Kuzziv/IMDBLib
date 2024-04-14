using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.People
{
    public class Profession
    {
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }

        public ICollection<PersonProfession> PersonProfessions { get; set; }
    }
}

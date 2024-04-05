﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.People
{
    public class PersonProfession
    {
        public string Nconst { get; set; }
        public int ProfessionId { get; set; }
        public Person Person { get; set; }
        public Profession Profession { get; set; }
    }
}

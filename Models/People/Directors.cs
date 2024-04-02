using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.People
{
    public class Directors
    {
        public static int _lastId = 0;

        [Key]
        public int Id { get; set; }


        [ForeignKey("Title_Crew")]
        public int Title_CrewId { get; set; }
        public Title_Crew Title_Crew { get; set; }

        [ForeignKey("Crew")]
        public string DirectorNconst { get; set; }
        public Crew Crew { get; set; }

        public Directors()
        {
            // Increment the static variable and assign it to the Id property
            Id = ++_lastId;
        }

    }
}

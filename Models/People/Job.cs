using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Models.People
{
    public class Job
    {
        public static int _lastId = 0;

        [Key]
        public int Id { get; set; }


        public string JobName { get; set; }

        public Job()
        {
            // Increment the static variable and assign it to the Id property
            Id = ++_lastId;
        }

        // Define the navigation property
        public ICollection<Title_Crew> Title_Crews { get; set; }

        // Define the navigation property
        public ICollection<Crew_Profession> Crew_Profession { get; set; }
    }
}

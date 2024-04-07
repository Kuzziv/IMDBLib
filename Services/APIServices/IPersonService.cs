using IMDBLib.Models.People;
using IMDBLib.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Services.APIServices
{
    public interface IPersonService
    {
        Task<PersonView> GetPersonByIdAsync(string nmconst);
        Task<IEnumerable<PersonView>> GetAllPersonsAsync(int page, int pageSize);
        Task AddPersonAsync(Person person);
        Task<IEnumerable<PersonView>> SearchPersonsByNameAsync(string searchName, int page, int pageSize);
    }
}

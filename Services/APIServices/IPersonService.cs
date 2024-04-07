using IMDBLib.DTO;
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
        Task<PersonDTO> GetPersonByNmconstAsync(string nmconst);
        Task<IEnumerable<PersonDTO>> GetAllPersonsAsync(int page, int pageSize);
        Task<bool> AddPersonAsync(PersonDTO person);
        Task<IEnumerable<PersonDTO>> SearchPersonsByNameAsync(string searchName, int page, int pageSize);
    }
}

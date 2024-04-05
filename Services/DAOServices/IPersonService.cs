using IMDBLib.DAO;
using IMDBLib.Models.People;
using IMDBLib.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Services.DAOServices
{
    public interface IPersonService
    {
        Task<bool> AddPerson(CrewDAO crewDAO);
        Task<List<PersonView>> GetPersonListByName(string searchString);
        Task<PersonView> GetPersonInfoByNconst(string nconst);
        Task UpdatePerson();
        Task DeletePerson();
        Task<IEnumerable<PersonView>> GetAllPersons(int pageNumber, int pageSize);
    }
}

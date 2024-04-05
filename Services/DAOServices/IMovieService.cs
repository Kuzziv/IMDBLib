using IMDBLib.DAO;
using IMDBLib.Models.Movie;
using IMDBLib.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Services.DAOServices
{
    public interface IMovieService
    {
        Task<bool> AddMovie(TitleDAO TitleDAO);
        Task<IEnumerable<MovieView>> GetMovieListByTitle(string searchString, int pageNumber, int pageSize);
        Task<bool> UpdateMovie(TitleDAO titleDAO);
        Task<bool> DeleteMovie(string tconst);
        Task<IEnumerable<MovieView>> GetAllMovies(int pageNumber, int pageSize);

    }
}

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
        Task AddMovie(MovieDAO movieDAO);
        Task<List<MovieDAO>> GetMovieListByTitle(string searchString);
        Task<MovieDAO> GetMovieInfoByTconst(string tconst);
        Task UpdateMovie(MovieDAO movieDAO);
        Task DeleteMovie(string tconst);
        Task<List<MovieView>> GetAllMovies();
        Task<List<Title>> GetTitles();

    }
}

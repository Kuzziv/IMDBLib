using IMDBLib.DTO;
using IMDBLib.Models.Movie;
using IMDBLib.Models.People;
using IMDBLib.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Services.APIServices
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieView>> SearchByMovieTitleAsync(string searchTitle, int page, int pageSize);
        Task<IEnumerable<MovieView>> GetAllMoviesAsync(int page, int pageSize);
        Task<MovieView> GetMovieByTconstAsync(string movieTconst);
        Task<bool> AddMovieAsync(MovieDTO movie);
        Task<bool> UpdateMovieAsync(string movieTconst, MovieDTO updatedMovie);
        Task<bool> DeleteMovieAsync(string movieTconst);
    }
}

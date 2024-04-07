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
        Task AddMovieAsync(Title movie);
        Task UpdateMovieAsync(string movieTconst, Title updatedMovie);
        Task DeleteMovieAsync(string movieTconst);
    }
}

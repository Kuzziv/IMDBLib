using IMDBLib.DAO;
using IMDBLib.DataBase;
using IMDBLib.Models.Movie;
using IMDBLib.Models.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.Services.DAOServices
{
    public class MovieService : IMovieService
    {
        private readonly MyDbContext _dbContext;

        public MovieService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task AddMovie(MovieDAO movieDAO)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMovie(string tconst)
        {
            throw new NotImplementedException();
        }

        public Task<MovieDAO> GetMovieInfoByTconst(string tconst)
        {
            throw new NotImplementedException();
        }

        public Task<List<MovieDAO>> GetMovieListByTitle(string searchString)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMovie(MovieDAO movieDAO)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MovieView>> GetAllMovies()
        {
            throw new NotImplementedException();
        }

        public async  Task<List<Title>> GetTitles()
        {
            var titles = await _dbContext.Titles.Take(10).ToListAsync();

            return titles;
        }
    }
}

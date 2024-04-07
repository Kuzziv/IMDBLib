using IMDBLib.DataBase;
using IMDBLib.Models.Movie;
using IMDBLib.Models.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMDBLib.Services.APIServices
{
    public class MovieService : IMovieService
    {
        private readonly IMDBDbContext _dbContext;

        public MovieService(IMDBDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<MovieView>> SearchByMovieTitleAsync(string searchTitle, int page, int pageSize)
        {
            try
            {
                var query = _dbContext.MovieViews
                    .Where(m => m.PrimaryTitle.Contains(searchTitle));

                var movies = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return movies;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while searching for movies by title: {ex}");
                throw new Exception("Failed to search for movies by title.", ex);
            }
        }

        public async Task<IEnumerable<MovieView>> GetAllMoviesAsync(int page, int pageSize)
        {
            try
            {
                var query = _dbContext.MovieViews;

                var movies = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return movies;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching all movies: {ex}");
                throw new Exception("Failed to fetch all movies.", ex);
            }
        }

        public async Task<MovieView> GetMovieByTconstAsync(string movieTconst)
        {
            try
            {
                var movie = await _dbContext.MovieViews
                    .FirstOrDefaultAsync(m => m.Tconst == movieTconst);

                return movie;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching movie by Tconst: {ex}");
                throw new Exception("Failed to fetch movie by Tconst.", ex);
            }
        }

        public async Task AddMovieAsync(Title movie)
        {
            try
            {
                _dbContext.Titles.Add(movie);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding movie: {ex}");
                throw new Exception("Failed to add movie.", ex);
            }
        }

        public async Task UpdateMovieAsync(string movieTconst, Title updatedMovie)
        {
            try
            {
                var existingMovie = await _dbContext.Titles.FindAsync(movieTconst);
                if (existingMovie != null)
                {
                    existingMovie.TitleType = updatedMovie.TitleType;
                    existingMovie.PrimaryTitle = updatedMovie.PrimaryTitle;
                    existingMovie.OriginalTitle = updatedMovie.OriginalTitle;
                    existingMovie.IsAdult = updatedMovie.IsAdult;
                    existingMovie.StartYear = updatedMovie.StartYear;
                    existingMovie.EndYear = updatedMovie.EndYear;
                    existingMovie.RuntimeMinutes = updatedMovie.RuntimeMinutes;

                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating movie: {ex}");
                throw new Exception("Failed to update movie.", ex);
            }
        }

        public async Task DeleteMovieAsync(string movieTconst)
        {
            try
            {
                var movie = await _dbContext.Titles.FindAsync(movieTconst);
                if (movie != null)
                {
                    _dbContext.Titles.Remove(movie);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting movie: {ex}");
                throw new Exception("Failed to delete movie.", ex);
            }
        }
    }
}

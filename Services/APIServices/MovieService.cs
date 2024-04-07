using IMDBLib.DataBase;
using IMDBLib.DTO;
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

        // Search movies by title asynchronously
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
                // Log error and throw exception
                Console.WriteLine($"An error occurred while searching for movies by title: {ex}");
                throw new Exception("Failed to search for movies by title.", ex);
            }
        }

        // Get all movies asynchronously
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
                // Log error and throw exception
                Console.WriteLine($"An error occurred while fetching all movies: {ex}");
                throw new Exception("Failed to fetch all movies.", ex);
            }
        }

        // Get movie by Tconst asynchronously
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
                // Log error and throw exception
                Console.WriteLine($"An error occurred while fetching movie by Tconst: {ex}");
                throw new Exception("Failed to fetch movie by Tconst.", ex);
            }
        }

        // Add a new movie asynchronously
        public async Task<bool> AddMovieAsync(MovieDTO movie)
        {
            try
            {
                // Create a new Title entity
                var title = new Title
                {
                    Tconst = await GetNewTconstasync(),
                    TitleType = movie.TitleType,
                    PrimaryTitle = movie.PrimaryTitle,
                    OriginalTitle = movie.OriginalTitle,
                    IsAdult = movie.IsAdult,
                    StartYear = movie.StartYear,
                    EndYear = movie.EndYear,
                    RuntimeMinutes = movie.RuntimeMinutes
                };

                // Add the Title entity to the database context
                _dbContext.Titles.Add(title);

                // Loop through genre names and associate them with the title
                foreach (var genreName in movie.GenreNames)
                {
                    // Check if genre exists in the database
                    var existingGenre = await _dbContext.Genres.FirstOrDefaultAsync(g => g.GenreName == genreName);
                    if (existingGenre == null)
                    {
                        // If genre does not exist, create a new one
                        existingGenre = new Genre
                        {
                            GenreName = genreName
                        };
                        _dbContext.Genres.Add(existingGenre);
                    }

                    // Associate the genre with the title
                    var titleGenre = new TitleGenre { Title = title, Genre = existingGenre };
                    _dbContext.TitleGenres.Add(titleGenre);
                }

                // Save changes to the database
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log error and throw exception
                Console.WriteLine($"An error occurred while adding movie: {ex}");
                throw new Exception("Failed to add movie.", ex);
            }
        }

        // Update existing movie asynchronously
        public async Task<bool> UpdateMovieAsync(string movieTconst, MovieDTO updatedMovie)
        {
            try
            {
                // Find the existing movie in the database
                var existingMovie = await _dbContext.Titles
                    .Include(m => m.TitleGenres)
                    .ThenInclude(tg => tg.Genre)
                    .FirstOrDefaultAsync(m => m.Tconst == movieTconst);

                // If movie doesn't exist, return false
                if (existingMovie == null)
                    return false;

                // Check if any changes have been made to the movie attributes
                bool anyChanges = existingMovie.TitleType != updatedMovie.TitleType ||
                                  existingMovie.PrimaryTitle != updatedMovie.PrimaryTitle ||
                                  existingMovie.OriginalTitle != updatedMovie.OriginalTitle ||
                                  existingMovie.IsAdult != updatedMovie.IsAdult ||
                                  existingMovie.StartYear != updatedMovie.StartYear ||
                                  existingMovie.EndYear != updatedMovie.EndYear ||
                                  existingMovie.RuntimeMinutes != updatedMovie.RuntimeMinutes;

                // Update movie attributes if changes were made
                if (anyChanges)
                {
                    existingMovie.TitleType = updatedMovie.TitleType;
                    existingMovie.PrimaryTitle = updatedMovie.PrimaryTitle;
                    existingMovie.OriginalTitle = updatedMovie.OriginalTitle;
                    existingMovie.IsAdult = updatedMovie.IsAdult;
                    existingMovie.StartYear = updatedMovie.StartYear;
                    existingMovie.EndYear = updatedMovie.EndYear;
                    existingMovie.RuntimeMinutes = updatedMovie.RuntimeMinutes;
                }

                // Update genres
                existingMovie.TitleGenres.Clear();
                foreach (var genreName in updatedMovie.GenreNames)
                {
                    var existingGenre = existingMovie.TitleGenres.FirstOrDefault(tg => tg.Genre.GenreName == genreName)?.Genre
                                        ?? new Genre { GenreName = genreName };
                    existingMovie.TitleGenres.Add(new TitleGenre { Title = existingMovie, Genre = existingGenre });
                }

                // Save changes to the database
                await _dbContext.SaveChangesAsync();

                // Return true indicating success
                return true;
            }
            catch (Exception ex)
            {
                // Log error and throw exception
                Console.WriteLine($"An error occurred while updating movie: {ex}");
                throw new Exception("Failed to update movie.", ex);
            }
        }

        // Delete movie by Tconst asynchronously
        public async Task<bool> DeleteMovieAsync(string movieTconst)
        {
            try
            {
                var movie = await _dbContext.Titles.FindAsync(movieTconst);
                if (movie != null)
                {
                    _dbContext.Titles.Remove(movie);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log error and throw exception
                Console.WriteLine($"An error occurred while deleting movie: {ex}");
                throw new Exception("Failed to delete movie.", ex);
            }
        }

        // Get the highest Tconst and increment it by 1 to generate a new Tconst asynchronously
        private async Task<string> GetNewTconstasync()
        {
            // get the highest tconst from the database
            var highestTconst = await _dbContext.Titles
                .OrderByDescending(t => t.Tconst)
                .FirstOrDefaultAsync();

            // increment the highest tconst by 1
            var newTconst = (int.Parse(highestTconst.Tconst.Substring(2)) + 1).ToString();
            return newTconst;
        }
    }
}

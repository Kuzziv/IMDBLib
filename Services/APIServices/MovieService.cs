using IMDBLib.DataBase;
using IMDBLib.DTO;
using IMDBLib.Models.Movie;
using IMDBLib.Models.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMDBLib.Services.APIServices
{
    public class MovieService : IMovieService
    {
        private readonly IMDBDbContext _dbContext;
        private readonly ILogger<MovieService> _logger;

        public MovieService(IMDBDbContext dbContext, ILogger<MovieService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger;
        }

        // Search movies by title asynchronously
        public async Task<IEnumerable<MovieDTO>> SearchByMovieTitleAsync(string searchTitle, int page, int pageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTitle))
                    throw new ArgumentException("Search title cannot be null or empty.");

                var query = _dbContext.MovieViews
                    .Where(m => m.PrimaryTitle.Contains(searchTitle));

                var movies = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (movies == null || movies.Count == 0)
                    return null;

                return MapMovieViewToMovieDto(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while searching for movies by title: {ex.Message}");
                throw;
            }
        }

        // Get all movies asynchronously
        public async Task<IEnumerable<MovieDTO>> GetAllMoviesAsync(int page, int pageSize)
        {
            try
            {
                var movies = await _dbContext.MovieViews
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return MapMovieViewToMovieDto(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching all movies: {ex.Message}");
                throw;
            }
        }

        // Get movie by Tconst asynchronously
        public async Task<MovieDTO> GetMovieByTconstAsync(string movieTconst)
        {
            try
            {
                if (string.IsNullOrEmpty(movieTconst))
                    throw new ArgumentException("Movie Tconst cannot be null or empty.");

                var movie = await _dbContext.MovieViews
                    .FirstOrDefaultAsync(m => m.Tconst == movieTconst);

                if (movie == null)
                    return null;

                return MapMovieViewToMovieDto(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching movie by Tconst: {ex.Message}");
                throw;
            }
        }

        // Add a new movie asynchronously
        public async Task<bool> AddMovieAsync(MovieDTO movie)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                if (movie == null)
                    throw new ArgumentNullException(nameof(movie), "Movie DTO cannot be null.");

                var title = new Title
                {
                    Tconst = await GetNewTconstAsync(),
                    TitleType = movie.TitleType,
                    PrimaryTitle = movie.PrimaryTitle,
                    OriginalTitle = movie.OriginalTitle,
                    IsAdult = movie.IsAdult,
                    StartYear = movie.StartYear,
                    EndYear = movie.EndYear,
                    RuntimeMinutes = movie.RuntimeMinutes
                };

                _dbContext.Titles.Add(title);

                foreach (var genreName in movie.GenreNames)
                {
                    if (string.IsNullOrWhiteSpace(genreName))
                        continue; // Skip empty or whitespace genre names

                    var existingGenre = await _dbContext.Genres
                        .FirstOrDefaultAsync(g => g.GenreName == genreName);

                    if (existingGenre == null)
                    {
                        existingGenre = new Genre { GenreName = genreName };
                        _dbContext.Genres.Add(existingGenre);
                    }

                    // Ensure TitleGenres collection is initialized
                    if (title.TitleGenres == null)
                    {
                        title.TitleGenres = new List<TitleGenre>();
                    }

                    title.TitleGenres.Add(new TitleGenre { Genre = existingGenre });
                }

                await _dbContext.SaveChangesAsync();

                // Commit the transaction if all operations succeed
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any operation fails
                await transaction.RollbackAsync();

                _logger.LogError(ex, $"An error occurred while adding movie: {ex.Message}");
                throw;
            }
        }


        // Update existing movie asynchronously
        public async Task<bool> UpdateMovieAsync(string movieTconst, MovieDTO updatedMovie)
        {
            try
            {                
                if (string.IsNullOrEmpty(movieTconst))
                    throw new ArgumentException("Movie Tconst cannot be null or empty.");

                var existingMovie = await _dbContext.Titles
                    .Include(m => m.TitleGenres)
                    .ThenInclude(tg => tg.Genre)
                    .FirstOrDefaultAsync(m => m.Tconst == movieTconst);

                if (existingMovie == null)
                    return false;

                MapMovieDtoToTitle(updatedMovie, existingMovie);

                existingMovie.TitleGenres.Clear();
                foreach (var genreName in updatedMovie.GenreNames)
                {
                    if (string.IsNullOrWhiteSpace(genreName))
                        continue; // Skip empty or whitespace genre names

                    var existingGenre = await _dbContext.Genres.FirstOrDefaultAsync(g => g.GenreName == genreName);
                    if (existingGenre == null)
                    {
                        existingGenre = new Genre { GenreName = genreName };
                        _dbContext.Genres.Add(existingGenre);
                    }

                    // Ensure TitleGenres collection is initialized
                    if (existingMovie.TitleGenres == null)
                        existingMovie.TitleGenres = new List<TitleGenre>();

                    existingMovie.TitleGenres.Add(new TitleGenre { Genre = existingGenre });
                }
                // Associate genres with title asynchronously

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating movie: {ex.Message}");
                throw;
            }
        }

        // Delete movie by Tconst asynchronously
        public async Task<bool> DeleteMovieAsync(string movieTconst)
        {
            try
            {
                if (string.IsNullOrEmpty(movieTconst))
                    throw new ArgumentException("Movie Tconst cannot be null or empty.");

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
                _logger.LogError(ex, $"An error occurred while deleting movie: {ex.Message}");
                throw;
            }
        }

        // Get the highest Tconst and increment it by 1 to generate a new Tconst asynchronously
        private async Task<string> GetNewTconstAsync()
        {
            var highestTconst = await _dbContext.Titles
                .OrderByDescending(t => t.Tconst)
                .FirstOrDefaultAsync();

            if (highestTconst == null)
            {
                // If no existing Tconst found, start with the minimum value (assuming "tt0000001")
                return "tt0000001";
            }
            else
            {
                // Parse the current Tconst number
                if (int.TryParse(highestTconst.Tconst.Substring(2), out int currentNumber))
                {
                    // Increment the number and format it with leading zeros
                    string newTconst = $"tt{currentNumber + 1:D7}";
                    return newTconst;
                }
                else
                {
                    // If parsing fails, handle the error accordingly
                    throw new InvalidOperationException("Unable to parse Tconst number.");
                }
            }
        }

        // Map MovieDTO properties to Title entity
        private void MapMovieDtoToTitle(MovieDTO movieDto, Title title)
        {        
            title.TitleType = movieDto.TitleType;
            title.PrimaryTitle = movieDto.PrimaryTitle;
            title.OriginalTitle = movieDto.OriginalTitle;
            title.IsAdult = movieDto.IsAdult;
            title.StartYear = movieDto.StartYear;
            title.EndYear = movieDto.EndYear;
            title.RuntimeMinutes = movieDto.RuntimeMinutes;
        }

        // Map MovieView properties to MovieDTO entity
        private MovieDTO MapMovieViewToMovieDto(MovieView movieView)
        {
            return new MovieDTO
            {
                Tconst = movieView.Tconst,
                TitleType = movieView.TitleType,
                PrimaryTitle = movieView.PrimaryTitle,
                OriginalTitle = movieView.OriginalTitle,
                IsAdult = movieView.IsAdult,
                StartYear = movieView.StartYear,
                EndYear = movieView.EndYear,
                RuntimeMinutes = movieView.RuntimeMinutes,
                GenreNames = movieView.GenreNames.Split(',').ToList()
            };
        }

        // Map MovieView properties to MovieDTO entity (overload for list)
        private List<MovieDTO> MapMovieViewToMovieDto(List<MovieView> moviesView)
        {
            return moviesView.Select(m => MapMovieViewToMovieDto(m)).ToList();
        }
    }
}

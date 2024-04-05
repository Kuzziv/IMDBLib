using IMDBLib.DAO;
using IMDBLib.DataBase;
using IMDBLib.Models.Movie;
using IMDBLib.Models.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IMDBLib.Services.DAOServices
{
    public class MovieService : IMovieService
    {
        private readonly MyDbContext _dbContext;

        public MovieService(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddMovie(TitleDAO titleDAO)
        {
            try
            {
                // look up the higest Tconst and increment it by 1
                var highestTconst = _dbContext.Titles.Max(t => t.Tconst);
                var newTconst = (int.Parse(highestTconst.Substring(2)) + 1).ToString("tt0000000");

                var title = new Title
                {
                    Tconst = newTconst,
                    PrimaryTitle = titleDAO.PrimaryTitle,
                    OriginalTitle = titleDAO.OriginalTitle,
                    IsAdult = titleDAO.IsAdult,
                    StartYear = titleDAO.StartYear,
                    EndYear = titleDAO.EndYear,
                    RuntimeMinutes = titleDAO.RuntimeMinutes,
                    Title_Genres = new List<Title_Genre>() // Initialize Title_Genres collection
                };

                // Look up or add TitleType
                var existingTitleType = _dbContext.TitleTypes.FirstOrDefault(tt => tt.Type == titleDAO.TitleType);
                if (existingTitleType == null)
                {
                    existingTitleType = new TitleType { Type = titleDAO.TitleType };
                    _dbContext.TitleTypes.Add(existingTitleType);
                }
                title.TitleType = existingTitleType;

                // Look up or add genres
                foreach (var genreName in titleDAO.Genres)
                {
                    var existingGenre = _dbContext.Genres.FirstOrDefault(g => g.GenreName == genreName);
                    if (existingGenre == null)
                    {
                        existingGenre = new Genre { GenreName = genreName };
                        _dbContext.Genres.Add(existingGenre);
                    }
                    // Add Title_Genre relationship
                    title.Title_Genres.Add(new Title_Genre { Genre = existingGenre });
                }

                // Check for existing Title with the same Tconst
                var existingTitle = _dbContext.Titles.FirstOrDefault(t => t.Tconst == title.Tconst);
                if (existingTitle != null)
                {
                    // Update existing Title if found
                    _dbContext.Entry(existingTitle).CurrentValues.SetValues(title);
                }
                else
                {
                    // Add the new movie to the database
                    _dbContext.Titles.Add(title);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                // Log the error or handle specific exceptions (e.g., unique constraint violation)
                // You may want to provide a more specific message based on the exception
                throw new Exception("Error in MovieService.AddMovie: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception("Error in MovieService.AddMovie", ex);
            }
        }






        public async Task<bool> DeleteMovie(string tconst)
        {
            try
            {
                // Find the movie by its tconst
                var movie = await _dbContext.Titles.FirstOrDefaultAsync(m => m.Tconst == tconst);

                if (movie != null)
                {
                    // Remove the movie from the database
                    _dbContext.Titles.Remove(movie);
                    await _dbContext.SaveChangesAsync();
                    return true; // Return true if deletion is successful
                }
                else
                {
                    return false; // Return false if movie not found
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in MovieService.DeleteMovie", ex);
            }
        }


        public async Task<IEnumerable<MovieView>> GetMovieListByTitle(string searchString, int pageNumber, int pageSize)
        {
            try
            {
                // Calculate the number of items to skip
                int skip = (pageNumber - 1) * pageSize;

                // Filter movies by primary title or original title containing the search string
                var query = _dbContext.MovieViews
                                      .Where(m => m.PrimaryTitle.Contains(searchString)
                                               || m.OriginalTitle.Contains(searchString))
                                      .Skip(skip)
                                      .Take(pageSize);

                // Execute the query and return the result
                var result = await query.ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in MovieService.GetMovieListByTitle", ex);
            }
        }


        public async Task<bool> UpdateMovie(TitleDAO titleDAO)
        {
            try
            {
                // Retrieve the movie from the database based on its Tconst
                var movieToUpdate = await _dbContext.Titles
                    .Include(t => t.TitleType)
                    .Include(t => t.Title_Genres)
                        .ThenInclude(tg => tg.Genre)
                    .FirstOrDefaultAsync(t => t.Tconst == titleDAO.Tconst);

                if (movieToUpdate != null)
                {
                    // Update the movie properties with the new values from TitleDAO
                    movieToUpdate.PrimaryTitle = titleDAO.PrimaryTitle;
                    movieToUpdate.OriginalTitle = titleDAO.OriginalTitle;
                    movieToUpdate.IsAdult = titleDAO.IsAdult;
                    movieToUpdate.StartYear = titleDAO.StartYear;
                    movieToUpdate.EndYear = titleDAO.EndYear;
                    movieToUpdate.RuntimeMinutes = titleDAO.RuntimeMinutes;

                    // Update TitleType
                    var existingTitleType = _dbContext.TitleTypes.FirstOrDefault(tt => tt.Type == titleDAO.TitleType);
                    if (existingTitleType == null)
                    {
                        existingTitleType = new TitleType { Type = titleDAO.TitleType };
                        _dbContext.TitleTypes.Add(existingTitleType);
                    }
                    movieToUpdate.TitleType = existingTitleType;

                    // Update Genres
                    foreach (var genreName in titleDAO.Genres)
                    {
                        var existingGenre = _dbContext.Genres.FirstOrDefault(g => g.GenreName == genreName);
                        if (existingGenre == null)
                        {
                            existingGenre = new Genre { GenreName = genreName };
                            _dbContext.Genres.Add(existingGenre);
                        }
                        // Check if the movie already has this genre
                        if (!movieToUpdate.Title_Genres.Any(tg => tg.Genre.GenreName == genreName))
                        {
                            // Add Title_Genre relationship
                            movieToUpdate.Title_Genres.Add(new Title_Genre { Genre = existingGenre });
                        }
                    }

                    // delete old genres
                    var genresToDelete = movieToUpdate.Title_Genres.Where(tg => !titleDAO.Genres.Contains(tg.Genre.GenreName)).ToList();
                    foreach (var tg in genresToDelete)
                    {
                        movieToUpdate.Title_Genres.Remove(tg);
                    }

                    // Save changes to the database
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    // Movie not found, return false
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions and return false
                throw new Exception("Error in MovieService.UpdateMovie", ex);
            }
        }


        public async Task<IEnumerable<MovieView>> GetAllMovies(int pageNumber, int pageSize)
        {
            try
            {
                // Get all movies
                var qurry = _dbContext.MovieViews.AsQueryable();

                // Calculate the number of items to skip
                int skip = (pageNumber - 1) * pageSize;

                // Use the extension method to retrieve the data
                var result = await qurry.Skip(skip).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in MovieService.GetAllMovies", ex);
            }
        }
    }
}

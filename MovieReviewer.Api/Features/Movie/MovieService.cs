using Microsoft.EntityFrameworkCore;
using MovieReviewer.Api.Data;
using MovieReviewer.Api.Shared;
using MovieReviewer.Api.Shared.Dtos;
using MovieReviewer.Api.Shared.Helpers;
using System.Security.Cryptography.X509Certificates;
using Ardalis.Result;

namespace MovieReviewer.Api.Features.Movie
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;
        private readonly OmDbClient _omDbClient;

        public MovieService(ApplicationDbContext context, OmDbClient omDbClient)
        {
            _context = context;
            _omDbClient = omDbClient;
        }

        public async Task<ResponseFromService<int>> CreateMovie(string imdbId)
        {
            var response = await _omDbClient.GetMovieDataFromExternalApi(imdbId);
            if (response.IsSuccess == false)
                return new ResponseFromService<int> { IsSuccess = response.IsSuccess, Errors = response.Errors };
            else
            {
                await _context.Movies.AddAsync(response.Data);
                await _context.SaveChangesAsync();
                return new ResponseFromService<int> { IsSuccess = response.IsSuccess, Data = response.Data.Id };
            }
        }

        public async Task<ResponseFromService> DeleteMovie(int movieId)
        {
            var response = await _context.Movies.FirstOrDefaultAsync(x => x.Id == movieId);

            if (response is null)
            {
                return new ResponseFromService
                    { IsSuccess = false, Errors = new List<string> { "Item isn't in the db" } };
            }

            response.IsDisabled = true;
            response.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new ResponseFromService { IsSuccess = true };
        }

        public async Task<ResponseFromService<IReadOnlyList<MovieDto>>> GetAllMovieData()
        {
            var allMovieData = await _context.Movies.Where(x => x.IsDisabled == false).Select(x => x.ToMovieDto())
                .ToListAsync();

            if (allMovieData.Count > 0)
                return new ResponseFromService<IReadOnlyList<MovieDto>> { IsSuccess = true, Data = allMovieData };

            return new ResponseFromService<IReadOnlyList<MovieDto>>
                { IsSuccess = false, Errors = new List<string> { "no data" } };
        }

        public async Task<ResponseFromService<MovieDto>> GetMovieData(int movieId)
        {
            var response = await _context.Movies.FirstOrDefaultAsync(x => x.Id == movieId);

            if (response is null)
            {
                return new ResponseFromService<MovieDto>
                    { IsSuccess = false, Errors = new List<string> { "No Movie exists" } };
            }

            if (response.IsDisabled)
            {
                return new ResponseFromService<MovieDto>
                    { IsSuccess = false, Errors = new List<string> { "No Movie exists" } };
            }

            return new ResponseFromService<MovieDto> { IsSuccess = true, Data = response.ToMovieDto() };
        }


        public async Task<Result> UpdateMovieData(int movieId, MovieDto movie)
        {
            var item = await _context.Movies.FirstOrDefaultAsync(x => x.Id == movieId);

            if (item is null)
            {
                return Result.Conflict();
            }

            item.LastUpdatedAt = DateTime.UtcNow;
            item.Title = movie.Title;
            item.MovieRating = movie.MovieRating;
            item.MovieLanguage = movie.MovieLanguage;
            item.ImdbRating = movie.ImdbRating;
            item.IsDisabled = movie.IsDeleted;

            await _context.SaveChangesAsync();

            return Result.Success();
        }
    }
}
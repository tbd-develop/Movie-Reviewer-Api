using Ardalis.Result;
using MovieReviewer.Api.Shared;
using MovieReviewer.Api.Shared.Dtos;

namespace MovieReviewer.Api.Features.Movie
{
    public interface IMovieService
    {
        public Task<ResponseFromService<int>> CreateMovie(string imdbId);
        public Task<ResponseFromService<MovieDto>> GetMovieData(int movieId);
        public Task<ResponseFromService<IReadOnlyList<MovieDto>>> GetAllMovieData();
        public Task<ResponseFromService> DeleteMovie(int movieId);
        Task<Result> UpdateMovieData(int movieId, MovieDto movie);
    }
}

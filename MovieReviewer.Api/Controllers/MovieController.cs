using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using MovieReviewer.Api.Features.Movie;
using MovieReviewer.Api.Shared.Dtos;

namespace MovieReviewer.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie(string imdbId)
        {
            var response = await _movieService.CreateMovie(imdbId);

            if (response.IsSuccess)
            {
                return Ok(new { response.Data });
            }

            return BadRequest(response.Errors);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _movieService.GetAllMovieData();

            if (items.IsSuccess)
            {
                return Ok(items.Data);
            }

            return BadRequest(items.Errors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieData(int id)
        {
            var response = await _movieService.GetMovieData(id);

            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }

            return BadRequest(response.Errors);
        }

        [HttpPut("{id}")]
        [TranslateResultToActionResult]
        public async Task<Result> UpdateMovieData(int id, MovieDto movieDto)
        {
            return await _movieService.UpdateMovieData(id, movieDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovieData(int id)
        {
            //
            var response = await _movieService.DeleteMovie(id);
            if (response.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Errors);
        }
    }
}
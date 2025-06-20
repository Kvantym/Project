using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    public class MovieController : Controller
    {
        private readonly AppDbConectin _conectin;
        private readonly UserManager<IdentityUser> _userManager;
        public MovieController(AppDbConectin conectin, UserManager<IdentityUser> userManager)
        {
            _conectin = conectin;
            _userManager = userManager;
        }
        public ActionResult Index()
        {
          
            return View();
        }

        [HttpGet]
        public IActionResult MovieAddPage()
        {
            return View("MovieAddPage");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MovieListPage()
        {
            var userId = _userManager.GetUserId(User);

            var movies = await _conectin.movies.Where(m=> m.UserId == userId).ToListAsync();
            return View("MovieListPage", movies);
        }
      
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddMovie(Movie movie, IFormFile Image)
        {
            var userId = _userManager.GetUserId(User);
            movie.UserId = userId;

            if (!ModelState.IsValid)
            {
              
                if (Image != null && Image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Image.CopyToAsync(memoryStream);
                        movie.ImageData = memoryStream.ToArray();
                        movie.ImageMimeType = Image.ContentType;
                    }
                }
                _conectin.movies.Add(movie);
                await _conectin.SaveChangesAsync();
                return RedirectToAction("Index", "Home", new { message = "Фільм додано" });
            }
            return View("MovieAddPage", movie);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _conectin.movies.FindAsync(id);
            if (movie != null)
            {
                _conectin.movies.Remove(movie);
                await _conectin.SaveChangesAsync();
                return RedirectToAction("Index", "Home", new { message = "Фільм видалено" });
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> EditMoviePage(int id)
        {
            var movie = await _conectin.movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);  // модель має бути не null і з даними з бази
        }


        [HttpPost]
        public async Task<IActionResult> EditMoviePage(Movie updateMovie, IFormFile? Image)
        {
            if (!ModelState.IsValid)
            {
                return View(updateMovie);
            }

            var movie = await _conectin.movies.FindAsync(updateMovie.Id);

            if (movie == null)
            {
                return NotFound();
            }

            movie.Title = updateMovie.Title;
            movie.Autor = updateMovie.Autor;
            movie.Genre = updateMovie.Genre;
            movie.Year = updateMovie.Year;
            movie.Rating = updateMovie.Rating;
            movie.Description = updateMovie.Description;

            if(Image != null && Image.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await Image.CopyToAsync(memoryStream);
                movie.ImageData = memoryStream.ToArray();
                movie.ImageMimeType = Image.ContentType;
            }
            await _conectin.SaveChangesAsync();

            return RedirectToAction("MovieListPage", "Movie", new { message = "Фільм відредаговано" });
        }

        [HttpGet]
        public async Task<IActionResult> SortMovie(string filterField, string filterValue, string sortOrderYear, string sortOrderRating, int minYear, double minRating)
        {
            var movies = _conectin.movies.AsQueryable();

            if(!string.IsNullOrEmpty(filterField)&& !string.IsNullOrEmpty(filterValue))
            {
                switch (filterField)
                {
                    case "Autor":
                        movies = movies.Where(m => m.Autor.Contains(filterValue.ToLower()));
                        break;
                    case "Title":
                        movies = movies.Where(m => m.Title.Contains(filterValue.ToLower()));
                        break;
                    case "Genre":
                        movies = movies.Where(m => m.Genre.Contains(filterValue.ToLower()));
                        break;
                }
            }

            movies = movies.Where(m => m.Year >= minYear && m.Rating >= minRating);

            if (sortOrderYear == "YearAsc") { movies = movies.OrderBy(m => m.Year); }
            if (sortOrderYear== "YearDesc") { movies = movies.OrderByDescending(m => m.Year); }

            if (sortOrderRating == "RatingAsc") { movies = movies.OrderBy(m => m.Rating); }
            if (sortOrderRating == "RatingDesc") { movies = movies.OrderByDescending(m => m.Rating); }

            ViewBag.filterField = filterField;
            ViewBag.filterValue = filterValue;
            ViewBag.sortOrderYear = sortOrderYear;
            ViewBag.sortOrderRating = sortOrderRating;
            ViewBag.minYear = minYear;
            ViewBag.minRating = minRating;

            return View("MovieListPage",movies.ToList());

        }


    }
}

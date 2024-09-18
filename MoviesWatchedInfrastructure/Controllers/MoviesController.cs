using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesWatchedDomain.Model;

namespace MoviesWatchedInfrastructure.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MoviesWatchedContext _context;

        public MoviesController(MoviesWatchedContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            var moviesWatchedContext = _context.Movies.Include(m => m.Language);
            return View(await moviesWatchedContext.ToListAsync());
        }
        

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        //для заповнення випадаючих списків мов, акторів та жанрів як при створенні/редагуванні
        private void PopulateDropDowns(Movie movie = null, List<int> selectedActors = null, List<int> selectedGenres = null)
        {
            // Мови
            ViewData["LanguageId"] = new SelectList(_context.Languages, "Id", "Name", movie?.LanguageId);

            // Актори
            var allActors = _context.Actors.ToList();
            var selectedActorsIds = selectedActors ?? movie?.MoviesActors.Select(ma => ma.ActorId).ToList() ?? new List<int>();
            ViewBag.Actors = allActors.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name,
                Selected = selectedActorsIds.Contains(a.Id)
            }).ToList();

            // Жанри
            var allGenres = _context.Genres.ToList();
            var selectedGenresIds = selectedGenres ?? movie?.MoviesGenres.Select(mg => mg.GenreId).ToList() ?? new List<int>();
            ViewBag.Genres = allGenres.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.Name,
                Selected = selectedGenresIds.Contains(g.Id)
            }).ToList();
        }


        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,LanguageId,Duration,MovieImage,ReviewDate")] Movie movie, int[] selectedActors, int[] selectedGenres)
        {
            if (ModelState.IsValid)
            {
                if (!await IsMovieExists(movie.Title, movie.ReleaseDate, movie.LanguageId, movie.Id))
                {
                    // Додаємо вибраних акторів і жанри до фільму
                    AddMovieActorsAndGenres(movie, selectedActors, selectedGenres);

                    _context.Add(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("LanguageId", "Фільм такого року та мовою перегляду вже створений.");
                }
            }

            PopulateDropDowns(movie, selectedActors.ToList(), selectedGenres.ToList());
            return View(movie);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MoviesActors)
                .Include(m => m.MoviesGenres)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            PopulateDropDowns(movie);
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,LanguageId,Duration,MovieImage,ReviewDate")] Movie movie, int[] selectedActors, int[] selectedGenres)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!await IsMovieExists(movie.Title, movie.ReleaseDate, movie.LanguageId, movie.Id))
                {
                    try
                    {
                        _context.Update(movie);
                        await _context.SaveChangesAsync();

                        await UpdateMovieActorsAndGenres(movie, selectedActors, selectedGenres);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MovieExists(movie.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                    ModelState.AddModelError("LanguageId", "Фільм такого року та мовою перегляду вже створений.");

            }

            PopulateDropDowns(movie, selectedActors.ToList(), selectedGenres.ToList());
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Знайти фільм разом з його зв'язаними даними
            var movie = await _context.Movies
                .Include(m => m.MoviesActors)
                .Include(m => m.MoviesGenres)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie != null)
            {
                // Видалити всі пов'язані актори
                var existingActors = _context.MoviesActors.Where(ma => ma.MovieId == movie.Id).ToList();
                _context.MoviesActors.RemoveRange(existingActors);

                // Видалити всі пов'язані жанри
                var existingGenres = _context.MoviesGenres.Where(mg => mg.MovieId == movie.Id).ToList();
                _context.MoviesGenres.RemoveRange(existingGenres);

                // Видалити сам фільм
                _context.Movies.Remove(movie);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        //для додавання зв'язків акторів та жанрів до фільму під час створення  
        private void AddMovieActorsAndGenres(Movie movie, int[] selectedActors, int[] selectedGenres)
        {
            foreach (var actorId in selectedActors)
            {
                movie.MoviesActors.Add(new MoviesActor { ActorId = actorId });
            }

            foreach (var genreId in selectedGenres)
            {
                movie.MoviesGenres.Add(new MoviesGenre { GenreId = genreId });
            }
        }

        //для оновлення акторів та жанрів під час редагування фільму
        private async Task UpdateMovieActorsAndGenres(Movie movie, int[] selectedActors, int[] selectedGenres)
        {
            // Оновлюємо список акторів
            var existingActors = _context.MoviesActors.Where(ma => ma.MovieId == movie.Id).ToList();
            _context.MoviesActors.RemoveRange(existingActors);
            foreach (var actorId in selectedActors)
            {
                _context.MoviesActors.Add(new MoviesActor { MovieId = movie.Id, ActorId = actorId });
            }

            // Оновлюємо список жанрів
            var existingGenres = _context.MoviesGenres.Where(mg => mg.MovieId == movie.Id).ToList();
            _context.MoviesGenres.RemoveRange(existingGenres);
            foreach (var genreId in selectedGenres)
            {
                _context.MoviesGenres.Add(new MoviesGenre { MovieId = movie.Id, GenreId = genreId });
            }

            await _context.SaveChangesAsync();
        }
        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
        private async Task<bool> IsMovieExists(string title, short releaseDate, int languageId, int id)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Title == title
                                       && m.ReleaseDate == releaseDate
                                       && m.LanguageId == languageId
                                       && m.Id != id);

            return movie != null;
        }

    }
}

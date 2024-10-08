﻿using System;
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
    public class GenresController : Controller
    {
        private readonly MoviesWatchedContext _context;

        public GenresController(MoviesWatchedContext context)
        {
            _context = context;
        }

        // GET: Genres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genres.ToListAsync());
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .Include(g => g.MoviesGenres)
                    .ThenInclude(mg => mg.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (genre == null)
            {
                return NotFound();
            }

            var movies = genre.MoviesGenres.Select(mg => mg.Movie).ToList();

            ViewBag.Genre = genre;
            ViewBag.Movies = movies;

            return View();
        }


        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,GenreImage")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                if (!await IsGenreExists(genre.Name, genre.Id))
                {
                    _context.Add(genre);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                    ModelState.AddModelError("Name", "Жанр з такою назвою вже існує.");

            }
            return View(genre);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,GenreImage")] Genre genre)
        {
            if (id != genre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!await IsGenreExists(genre.Name, genre.Id))
                {
                    try
                    {
                        _context.Update(genre);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!GenreExists(genre.Id))
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
                    ModelState.AddModelError("Name", "Жанр з такою назвою вже існує.");

            }
            return View(genre);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                return NotFound();


            if (await IsGenreLinkedToMovies(id))
                return Json(new { success = false, message = "Цей жанр пов'язаний з фільмами та не може бути видалений." });


            await DeleteGenre(genre);
            return Json(new { success = true, message = "Жанр успішно видалений." });
        }

        private async Task<bool> IsGenreLinkedToMovies(int genreId)
        {
            return await _context.MoviesGenres.AnyAsync(mg => mg.GenreId == genreId);
        }

        private async Task DeleteGenre(Genre genre)
        {
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }



        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
        private async Task<bool> IsGenreExists(string name, int id)
        {
            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.Name == name 
                                       && m.Id != id);

            return genre != null;
        }
    }
}

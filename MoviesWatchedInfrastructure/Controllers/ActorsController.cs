﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesWatchedDomain.Model;

namespace MoviesWatchedInfrastructure.Controllers
{
    public class ActorsController : Controller
    {
        private readonly MoviesWatchedContext _context;

        public ActorsController(MoviesWatchedContext context)
        {
            _context = context;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actors.ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();


            var actor = await _context.Actors
                .Include(a => a.MoviesActors) 
                    .ThenInclude(ma => ma.Movie) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (actor == null)
                return NotFound();


            var movies = actor.MoviesActors.Select(ma => ma.Movie).ToList();

            ViewBag.Actor = actor;
            ViewBag.Movies = movies;

            return View(actor);
        }


        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ActorImage")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                if (!await IsActorExists(actor.Name, actor.Id))
                {
                    _context.Add(actor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                    ModelState.AddModelError("Name", "Актор з таким іменем вже створений.");
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();


            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
                return NotFound();

            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ActorImage")] Actor actor)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!await IsActorExists(actor.Name, actor.Id))
                {
                    try
                    {
                        _context.Update(actor);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ActorExists(actor.Id))
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
                    ModelState.AddModelError("Name", "Актор з таким іменем вже створений.");
            }


            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();


            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
                return NotFound();


            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
                return NotFound();


            if (await IsActorLinkedToMovies(id))
                return Json(new { success = false, message = "Цей актор пов'язаний з фільмами та не може бути видалений." });


            await DeleteActor(actor);
            return Json(new { success = true, message = "Актор успішно видалений." });
        }

        private async Task<bool> IsActorLinkedToMovies(int actorId)
        {
            return await _context.MoviesActors.AnyAsync(ma => ma.ActorId == actorId);
        }

        private async Task DeleteActor(Actor actor)
        {
            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();
        }





        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }

        private async Task<bool> IsActorExists(string name, int id)
        {
            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.Name == name 
                                       && m.Id != id);

            return actor != null;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CitasMedicasApp.Data;
using CitasMedicasApp.Models;

namespace CitasMedicasApp.Controllers
{
    public class EspecialidadesController : Controller
    {
        private readonly CitasMedicasContext _context;

        public EspecialidadesController(CitasMedicasContext context)
        {
            _context = context;
        }

        // GET: Especialidades
        public async Task<IActionResult> Index()
        {
            return View(await _context.Especialidades.ToListAsync());
        }

        // GET: Especialidades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var especialidade = await _context.Especialidades
                .FirstOrDefaultAsync(m => m.EspecialidadId == id);
            if (especialidade == null)
            {
                return NotFound();
            }

            return View(especialidade);
        }

        // GET: Especialidades/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Especialidades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EspecialidadId,Nombre,Descripcion")] Especialidade especialidade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(especialidade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(especialidade);
        }

        // GET: Especialidades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var especialidade = await _context.Especialidades.FindAsync(id);
            if (especialidade == null)
            {
                return NotFound();
            }
            return View(especialidade);
        }

        // POST: Especialidades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EspecialidadId,Nombre,Descripcion")] Especialidade especialidade)
        {
            if (id != especialidade.EspecialidadId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(especialidade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EspecialidadeExists(especialidade.EspecialidadId))
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
            return View(especialidade);
        }

        // GET: Especialidades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var especialidade = await _context.Especialidades
                .FirstOrDefaultAsync(m => m.EspecialidadId == id);
            if (especialidade == null)
            {
                return NotFound();
            }

            return View(especialidade);
        }

        // POST: Especialidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var especialidade = await _context.Especialidades.FindAsync(id);
            if (especialidade != null)
            {
                _context.Especialidades.Remove(especialidade);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EspecialidadeExists(int id)
        {
            return _context.Especialidades.Any(e => e.EspecialidadId == id);
        }
    }
}

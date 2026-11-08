using CitasMedicasApp.Data;
using CitasMedicasApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicasApp.Controllers
{
    public class CitasController : Controller
    {
        private readonly CitasMedicasContext _context;
        public CitasController(CitasMedicasContext context) { _context = context; }

        public async Task<IActionResult> Index()
        {
            var citas = _context.Citas.Include(c => c.Consultorio).Include(c => c.Medico)
                .Include(c => c.Paciente).OrderBy(c => c.FechaHora);
            return View(await citas.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var cita = await _context.Citas.Include(c => c.Consultorio).Include(c => c.Medico)
                .Include(c => c.Paciente).FirstOrDefaultAsync(m => m.CitaId == id);
            return cita == null ? NotFound() : View(cita);
        }

        public IActionResult Create()
        {
            var cita = new Cita { Estado = "Programada", DuracionMin = 30 };
            CargarListas(cita);
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PacienteId,MedicoId,ConsultorioId,FechaHora,DuracionMin,Estado,Motivo")] Cita cita)
        {
            QuitarNavegacion();
            if (cita.FechaHora != default && cita.FechaHora < DateTime.Now)
                ModelState.AddModelError("FechaHora", "No se puede agendar una cita en el pasado.");
            await ValidarChoque(cita);
            if (ModelState.IsValid)
            {
                _context.Add(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            CargarListas(cita);
            return View(cita);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null) return NotFound();
            CargarListas(cita);
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CitaId,PacienteId,MedicoId,ConsultorioId,FechaHora,DuracionMin,Estado,Motivo")] Cita cita)
        {
            if (id != cita.CitaId) return NotFound();
            QuitarNavegacion();
            await ValidarChoque(cita);
            if (ModelState.IsValid)
            {
                _context.Update(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            CargarListas(cita);
            return View(cita);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var cita = await _context.Citas.Include(c => c.Consultorio).Include(c => c.Medico)
                .Include(c => c.Paciente).FirstOrDefaultAsync(m => m.CitaId == id);
            return cita == null ? NotFound() : View(cita);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null) _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void QuitarNavegacion()
        {
            ModelState.Remove("Paciente");
            ModelState.Remove("Medico");
            ModelState.Remove("Consultorio");
            ModelState.Remove("Consulta");
        }

        private async Task ValidarChoque(Cita cita)
        {
            bool existe = await _context.Citas.AnyAsync(c => c.MedicoId == cita.MedicoId
                && c.FechaHora == cita.FechaHora && c.CitaId != cita.CitaId);
            if (existe) ModelState.AddModelError("FechaHora", "Ese médico ya tiene una cita a esa hora.");
        }

        private void CargarListas(Cita cita)
        {
            ViewBag.Pacientes = new SelectList(
                _context.Pacientes.OrderBy(p => p.Apellidos)
                    .Select(p => new { p.PacienteId, Nombre = p.Nombres + " " + p.Apellidos }).ToList(),
                "PacienteId", "Nombre", cita.PacienteId);
            ViewBag.Medicos = new SelectList(
                _context.Medicos.OrderBy(m => m.Apellidos)
                    .Select(m => new { m.MedicoId, Nombre = m.Nombres + " " + m.Apellidos }).ToList(),
                "MedicoId", "Nombre", cita.MedicoId);
            ViewBag.Consultorios = new SelectList(
                _context.Consultorios.OrderBy(c => c.Numero).ToList(),
                "ConsultorioId", "Numero", cita.ConsultorioId);
        }
    }
}

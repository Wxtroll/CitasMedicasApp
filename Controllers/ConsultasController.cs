using CitasMedicasApp.Data;
using CitasMedicasApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicasApp.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly CitasMedicasContext _context;
        public ConsultasController(CitasMedicasContext context) { _context = context; }

        public async Task<IActionResult> Index()
        {
            var lista = await _context.Consultas
                .Include(c => c.Cita).ThenInclude(c => c.Paciente)
                .Include(c => c.Cita).ThenInclude(c => c.Medico)
                .OrderByDescending(c => c.FechaRegistro)
                .ToListAsync();
            return View(lista);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var consulta = await _context.Consultas
                .Include(c => c.Cita).ThenInclude(c => c.Paciente)
                .Include(c => c.Cita).ThenInclude(c => c.Medico)
                .FirstOrDefaultAsync(c => c.ConsultaId == id);
            if (consulta == null) return NotFound();
            ViewBag.Recetas = await _context.Recetas
                .Include(r => r.Medicamento)
                .Where(r => r.ConsultaId == id)
                .ToListAsync();
            return View(consulta);
        }

        public async Task<IActionResult> Atender(int? citaId)
        {
            if (citaId == null) return NotFound();
            var cita = await _context.Citas.Include(c => c.Paciente).Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.CitaId == citaId);
            if (cita == null) return NotFound();

            var existente = await _context.Consultas.FirstOrDefaultAsync(c => c.CitaId == citaId);
            if (existente != null)
                return RedirectToAction(nameof(Details), new { id = existente.ConsultaId });

            if (cita.Estado == "Cancelada")
            {
                TempData["Error"] = "No se puede atender una cita cancelada.";
                return RedirectToAction("Index", "Citas");
            }

            var vm = new AtenderViewModel { CitaId = cita.CitaId };
            LlenarDatosCita(vm, cita);
            CargarMedicamentos();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Atender(AtenderViewModel vm)
        {
            var cita = await _context.Citas.Include(c => c.Paciente).Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.CitaId == vm.CitaId);
            if (cita == null) return NotFound();
            LlenarDatosCita(vm, cita);

            if (cita.Estado == "Cancelada")
                ModelState.AddModelError("", "No se puede atender una cita cancelada.");
            if (await _context.Consultas.AnyAsync(c => c.CitaId == vm.CitaId))
                ModelState.AddModelError("", "Esta cita ya fue atendida.");

            var recetas = vm.Recetas.Where(r => r.MedicamentoId != null).ToList();
            foreach (var r in recetas)
            {
                if (string.IsNullOrWhiteSpace(r.Dosis) || string.IsNullOrWhiteSpace(r.Frecuencia)
                    || r.DuracionDias is null or <= 0)
                {
                    ModelState.AddModelError("", "Cada medicamento necesita dosis, frecuencia y duración en días.");
                    break;
                }
            }
            if (recetas.GroupBy(r => r.MedicamentoId).Any(g => g.Count() > 1))
                ModelState.AddModelError("", "No repitas el mismo medicamento en la receta.");

            if (!ModelState.IsValid)
            {
                CargarMedicamentos();
                return View(vm);
            }

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var consulta = new Consulta
                {
                    CitaId = vm.CitaId,
                    Diagnostico = vm.Diagnostico,
                    Observaciones = vm.Observaciones
                };
                _context.Consultas.Add(consulta);
                await _context.SaveChangesAsync();

                foreach (var r in recetas)
                {
                    _context.Recetas.Add(new Receta
                    {
                        ConsultaId = consulta.ConsultaId,
                        MedicamentoId = r.MedicamentoId!.Value,
                        Dosis = r.Dosis!,
                        Frecuencia = r.Frecuencia!,
                        DuracionDias = r.DuracionDias!.Value
                    });
                }

                cita.Estado = "Atendida";
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return RedirectToAction(nameof(Details), new { id = consulta.ConsultaId });
            }
            catch
            {
                await tx.RollbackAsync();
                ModelState.AddModelError("", "No se pudo guardar la consulta. Intenta de nuevo.");
                CargarMedicamentos();
                return View(vm);
            }
        }

        private static void LlenarDatosCita(AtenderViewModel vm, Cita cita)
        {
            vm.Paciente = cita.Paciente.Nombres + " " + cita.Paciente.Apellidos;
            vm.Medico = cita.Medico.Nombres + " " + cita.Medico.Apellidos;
            vm.FechaHora = cita.FechaHora;
        }

        private void CargarMedicamentos()
        {
            ViewBag.Medicamentos = _context.Medicamentos.OrderBy(m => m.Nombre)
                .Select(m => new SelectListItem { Value = m.MedicamentoId.ToString(), Text = m.Nombre })
                .ToList();
        }
    }
}

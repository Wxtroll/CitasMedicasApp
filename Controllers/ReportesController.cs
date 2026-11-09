using CitasMedicasApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicasApp.Controllers
{
    public class ResumenItem { public string Nombre { get; set; } = ""; public int Total { get; set; } }

    public class HistorialItem
    {
        public DateTime FechaHora { get; set; }
        public string Medico { get; set; } = "";
        public string Especialidad { get; set; } = "";
        public string Estado { get; set; } = "";
        public string? Diagnostico { get; set; }
    }

    public class ReportesViewModel
    {
        public List<ResumenItem> PorEspecialidad { get; set; } = new();
        public List<ResumenItem> TopMedicamentos { get; set; } = new();
        public List<HistorialItem>? Historial { get; set; }
        public int? PacienteId { get; set; }
        public List<SelectListItem> Pacientes { get; set; } = new();
    }

    public class ReportesController : Controller
    {
        private readonly CitasMedicasContext _context;
        public ReportesController(CitasMedicasContext context) { _context = context; }

        public async Task<IActionResult> Index(int? pacienteId)
        {
            var vm = new ReportesViewModel { PacienteId = pacienteId };

            vm.PorEspecialidad = await _context.Citas
                .GroupBy(c => c.Medico.Especialidad.Nombre)
                .Select(g => new ResumenItem { Nombre = g.Key, Total = g.Count() })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            vm.TopMedicamentos = await _context.Recetas
                .GroupBy(r => r.Medicamento.Nombre)
                .Select(g => new ResumenItem { Nombre = g.Key, Total = g.Count() })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToListAsync();

            vm.Pacientes = await _context.Pacientes
                .OrderBy(p => p.Apellidos)
                .Select(p => new SelectListItem
                {
                    Value = p.PacienteId.ToString(),
                    Text = p.Nombres + " " + p.Apellidos,
                    Selected = p.PacienteId == pacienteId
                })
                .ToListAsync();

            if (pacienteId != null)
            {
                vm.Historial = await _context.Citas
                    .Where(c => c.PacienteId == pacienteId)
                    .OrderByDescending(c => c.FechaHora)
                    .Select(c => new HistorialItem
                    {
                        FechaHora = c.FechaHora,
                        Medico = c.Medico.Nombres + " " + c.Medico.Apellidos,
                        Especialidad = c.Medico.Especialidad.Nombre,
                        Estado = c.Estado,
                        Diagnostico = c.Consulta != null ? c.Consulta.Diagnostico : null
                    })
                    .ToListAsync();
            }

            return View(vm);
        }
    }
}

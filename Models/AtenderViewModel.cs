using System.ComponentModel.DataAnnotations;

namespace CitasMedicasApp.Models
{
    public class RecetaItem
    {
        public int? MedicamentoId { get; set; }
        public string? Dosis { get; set; }
        public string? Frecuencia { get; set; }
        public int? DuracionDias { get; set; }
    }

    public class AtenderViewModel
    {
        public int CitaId { get; set; }
        public string? Paciente { get; set; }
        public string? Medico { get; set; }
        public DateTime FechaHora { get; set; }

        [Required(ErrorMessage = "El diagnóstico es obligatorio.")]
        [StringLength(500, ErrorMessage = "El diagnóstico admite máximo 500 caracteres.")]
        public string Diagnostico { get; set; } = "";

        public string? Observaciones { get; set; }
        public List<RecetaItem> Recetas { get; set; } = new();
    }
}

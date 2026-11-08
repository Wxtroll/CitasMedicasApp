using System;
using System.Collections.Generic;

namespace CitasMedicasApp.Models;

public partial class Consulta
{
    public int ConsultaId { get; set; }

    public int CitaId { get; set; }

    public string Diagnostico { get; set; } = null!;

    public string? Observaciones { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual Cita Cita { get; set; } = null!;

    public virtual ICollection<Receta> Receta { get; set; } = new List<Receta>();
}

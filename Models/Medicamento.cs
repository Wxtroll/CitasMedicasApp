using System;
using System.Collections.Generic;

namespace CitasMedicasApp.Models;

public partial class Medicamento
{
    public int MedicamentoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Presentacion { get; set; }

    public virtual ICollection<Receta> Receta { get; set; } = new List<Receta>();
}

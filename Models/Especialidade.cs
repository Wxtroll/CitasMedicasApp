using System;
using System.Collections.Generic;

namespace CitasMedicasApp.Models;

public partial class Especialidade
{
    public int EspecialidadId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Medico> Medicos { get; set; } = new List<Medico>();
}

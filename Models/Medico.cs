using System;
using System.Collections.Generic;

namespace CitasMedicasApp.Models;

public partial class Medico
{
    public int MedicoId { get; set; }

    public int EspecialidadId { get; set; }

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string NumeroJvpm { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual Especialidade Especialidad { get; set; } = null!;
}

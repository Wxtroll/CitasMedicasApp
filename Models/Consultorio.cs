using System;
using System.Collections.Generic;

namespace CitasMedicasApp.Models;

public partial class Consultorio
{
    public int ConsultorioId { get; set; }

    public string Numero { get; set; } = null!;

    public int Piso { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();
}

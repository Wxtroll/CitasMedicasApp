using System;
using System.Collections.Generic;

namespace CitasMedicasApp.Models;

public partial class Cita
{
    public int CitaId { get; set; }

    public int PacienteId { get; set; }

    public int MedicoId { get; set; }

    public int ConsultorioId { get; set; }

    public DateTime FechaHora { get; set; }

    public int DuracionMin { get; set; }

    public string Estado { get; set; } = null!;

    public string? Motivo { get; set; }

    public virtual Consulta? Consulta { get; set; }

    public virtual Consultorio Consultorio { get; set; } = null!;

    public virtual Medico Medico { get; set; } = null!;

    public virtual Paciente Paciente { get; set; } = null!;
}

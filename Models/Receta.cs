using System;
using System.Collections.Generic;

namespace CitasMedicasApp.Models;

public partial class Receta
{
    public int ConsultaId { get; set; }

    public int MedicamentoId { get; set; }

    public string Dosis { get; set; } = null!;

    public string Frecuencia { get; set; } = null!;

    public int DuracionDias { get; set; }

    public virtual Consulta Consulta { get; set; } = null!;

    public virtual Medicamento Medicamento { get; set; } = null!;
}

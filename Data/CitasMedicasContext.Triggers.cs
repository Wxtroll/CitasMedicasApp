using CitasMedicasApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicasApp.Data
{
    public partial class CitasMedicasContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cita>().ToTable(tb => tb.HasTrigger("trg_Citas_Auditoria"));
            modelBuilder.Entity<Consulta>().ToTable(tb => tb.HasTrigger("trg_Consultas_ValidarCita"));
        }
    }
}

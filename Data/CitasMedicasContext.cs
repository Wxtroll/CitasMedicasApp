using System;
using System.Collections.Generic;
using CitasMedicasApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicasApp.Data;

public partial class CitasMedicasContext : DbContext
{
    public CitasMedicasContext()
    {
    }

    public CitasMedicasContext(DbContextOptions<CitasMedicasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cita> Citas { get; set; }

    public virtual DbSet<Consulta> Consultas { get; set; }

    public virtual DbSet<Consultorio> Consultorios { get; set; }

    public virtual DbSet<Especialidade> Especialidades { get; set; }

    public virtual DbSet<Medicamento> Medicamentos { get; set; }

    public virtual DbSet<Medico> Medicos { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<Receta> Recetas { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.CitaId).HasName("PK__Citas__F0E2D9D2D4879E3B");

            entity.HasIndex(e => new { e.MedicoId, e.FechaHora }, "UX_Citas_Medico_FechaHora").IsUnique();

            entity.Property(e => e.DuracionMin).HasDefaultValue(30);
            entity.Property(e => e.Estado)
                .HasMaxLength(15)
                .HasDefaultValue("Programada");
            entity.Property(e => e.Motivo).HasMaxLength(200);

            entity.HasOne(d => d.Consultorio).WithMany(p => p.Cita)
                .HasForeignKey(d => d.ConsultorioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citas_Consultorios");

            entity.HasOne(d => d.Medico).WithMany(p => p.Cita)
                .HasForeignKey(d => d.MedicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citas_Medicos");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Cita)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citas_Pacientes");
        });

        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.HasKey(e => e.ConsultaId).HasName("PK__Consulta__7D0B7DCC8206965A");

            entity.HasIndex(e => e.CitaId, "UQ__Consulta__F0E2D9D335FAC1B1").IsUnique();

            entity.Property(e => e.Diagnostico).HasMaxLength(500);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Cita).WithOne(p => p.Consulta)
                .HasForeignKey<Consulta>(d => d.CitaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consultas_Citas");
        });

        modelBuilder.Entity<Consultorio>(entity =>
        {
            entity.HasKey(e => e.ConsultorioId).HasName("PK__Consulto__9E5B8A3F7B2FBE27");

            entity.HasIndex(e => e.Numero, "UQ__Consulto__7E532BC64A0E9A3A").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Numero).HasMaxLength(20);
        });

        modelBuilder.Entity<Especialidade>(entity =>
        {
            entity.HasKey(e => e.EspecialidadId).HasName("PK__Especial__F9071831FDD22038");

            entity.HasIndex(e => e.Nombre, "UQ__Especial__75E3EFCF08067B22").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(80);
        });

        modelBuilder.Entity<Medicamento>(entity =>
        {
            entity.HasKey(e => e.MedicamentoId).HasName("PK__Medicame__003D65D3A83B8165");

            entity.HasIndex(e => e.Nombre, "UQ__Medicame__75E3EFCF4C714B31").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Presentacion).HasMaxLength(60);
        });

        modelBuilder.Entity<Medico>(entity =>
        {
            entity.HasKey(e => e.MedicoId).HasName("PK__Medicos__5953C25654E05FE0");

            entity.HasIndex(e => e.NumeroJvpm, "UQ__Medicos__57556F86B68B08EE").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Apellidos).HasMaxLength(80);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.Nombres).HasMaxLength(80);
            entity.Property(e => e.NumeroJvpm).HasMaxLength(20);
            entity.Property(e => e.Telefono).HasMaxLength(15);

            entity.HasOne(d => d.Especialidad).WithMany(p => p.Medicos)
                .HasForeignKey(d => d.EspecialidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Medicos_Especialidades");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.PacienteId).HasName("PK__Paciente__9353C01FA0CBE15B");

            entity.HasIndex(e => e.Dui, "UX_Pacientes_Dui")
                .IsUnique()
                .HasFilter("([Dui] IS NOT NULL)");

            entity.Property(e => e.Apellidos).HasMaxLength(80);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Dui).HasMaxLength(10);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Nombres).HasMaxLength(80);
            entity.Property(e => e.Sexo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Telefono).HasMaxLength(15);
        });

        modelBuilder.Entity<Receta>(entity =>
        {
            entity.HasKey(e => new { e.ConsultaId, e.MedicamentoId });

            entity.Property(e => e.Dosis).HasMaxLength(60);
            entity.Property(e => e.Frecuencia).HasMaxLength(60);

            entity.HasOne(d => d.Consulta).WithMany(p => p.Receta)
                .HasForeignKey(d => d.ConsultaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Recetas_Consultas");

            entity.HasOne(d => d.Medicamento).WithMany(p => p.Receta)
                .HasForeignKey(d => d.MedicamentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Recetas_Medicamentos");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

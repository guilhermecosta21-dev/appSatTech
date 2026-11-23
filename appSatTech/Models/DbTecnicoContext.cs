using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace appSatTech.Models;

public partial class DbTecnicoContext : DbContext
{
    public DbTecnicoContext()
    {
    }

    public DbTecnicoContext(DbContextOptions<DbTecnicoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chamado> Chamados { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Tecnico> Tecnicos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConexaoSqlServer");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chamado>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__Chamado__06370DADF1E607D3");

            entity.ToTable("Chamado");

            entity.Property(e => e.DataHora).HasColumnType("datetime");
            entity.Property(e => e.StatusConsulta)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Chamados)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chamado_Cliente");

            entity.HasOne(d => d.Tecnico).WithMany(p => p.Chamados)
                .HasForeignKey(d => d.TecnicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chamado_Tecnico");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__Cliente__06370DADFE3197EC");

            entity.ToTable("Cliente");

            entity.HasIndex(e => e.Cpf, "UQ__Cliente__C1FF9309F32192E5").IsUnique();

            entity.Property(e => e.Cpf)
                .HasMaxLength(14)
                .IsUnicode(false);
            entity.Property(e => e.DataNascimento).HasColumnType("date");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tecnico>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__Tecnico__06370DAD5B560536");

            entity.ToTable("Tecnico");

            entity.HasIndex(e => e.RegistroTecnico, "UQ__Tecnico__58ABF294B22A69C5").IsUnique();

            entity.Property(e => e.Especialidade)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RegistroTecnico)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

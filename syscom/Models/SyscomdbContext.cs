using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace syscom.Models;

public partial class SyscomdbContext : DbContext
{
    public SyscomdbContext()
    {
    }

    public SyscomdbContext(DbContextOptions<SyscomdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
//arning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
  // => optionsBuilder.UseSqlServer("server=Asus_Santiago\\SQLEXPRESS; database=SYSCOMDB; integrated security=true; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC070F4CFC00");

            entity.Property(e => e.NombreCargo)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre_cargo");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC074AC9C059");

            entity.HasIndex(e => e.CorreoElectronico, "UQ__Usuarios__5B8A068296A03D08").IsUnique();

            entity.Property(e => e.Contrato)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("contrato");
            entity.Property(e => e.CorreoElectronico)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("correo_electronico");
            entity.Property(e => e.FechaEliminacion).HasColumnName("fecha_eliminacion");
            entity.Property(e => e.FechaIngreso).HasColumnName("fecha_ingreso");
            entity.Property(e => e.Firma)
                .HasColumnType("text")
                .HasColumnName("firma");
            entity.Property(e => e.IdRol).HasColumnName("Id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__Id_rol__4CA06362");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

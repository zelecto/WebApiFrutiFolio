using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebApiFrutiFolio.Models;

namespace WebApiFrutiFolio.Context;

public partial class FruityFolioContext : DbContext
{
    public FruityFolioContext()
    {
    }

    public FruityFolioContext(DbContextOptions<FruityFolioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<DetallesProductosVendido> DetallesProductosVendidos { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=ep-odd-recipe-a5janyq7.us-east-2.aws.neon.fl0.io;Database=FruityFolio;Username=fl0user;Password=Z4sUG0dhImCX");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Cedula).HasName("cliente_pkey");

            entity.ToTable("cliente");

            entity.Property(e => e.Cedula)
                .ValueGeneratedNever()
                .HasColumnName("cedula");
            entity.Property(e => e.Correo)
                .HasMaxLength(255)
                .HasColumnName("correo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<DetallesProductosVendido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("detalles_productos_vendidos_pkey");

            entity.ToTable("detalles_productos_vendidos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cantidadvendida).HasColumnName("cantidadvendida");
            entity.Property(e => e.Idfactura).HasColumnName("idfactura");
            entity.Property(e => e.Idproducto).HasColumnName("idproducto");
            entity.Property(e => e.Subprecio)
                .HasPrecision(10, 2)
                .HasColumnName("subprecio");

            entity.HasOne(d => d.IdfacturaNavigation).WithMany(p => p.DetallesProductosVendidos)
                .HasForeignKey(d => d.Idfactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalles_productos_vendidos_idfactura_fkey");

            entity.HasOne(d => d.producto).WithMany(p => p.DetallesProductosVendidos)
                .HasForeignKey(d => d.Idproducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalles_productos_vendidos_idproducto_fkey");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("factura_pkey");

            entity.ToTable("factura");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClienteCedula).HasColumnName("cliente_cedula");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Preciototal)
                .HasPrecision(10, 2)
                .HasColumnName("preciototal");
            entity.Property(e => e.UsuarioUsername)
                .HasMaxLength(50)
                .HasColumnName("usuario_username");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.ClienteCedula)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("factura_cliente_cedula_fkey");

            entity.HasOne(d => d.UsuarioUsernameNavigation).WithMany(p => p.Facturas)
                .HasForeignKey(d => d.UsuarioUsername)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("factura_usuario_username_fkey");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("productos_pkey");

            entity.ToTable("productos");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Img)
                .HasMaxLength(255)
                .HasColumnName("img");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            // Nueva propiedad "Activo"
            entity.Property(e => e.Activo).HasColumnName("activo").IsRequired().HasDefaultValue(true);

            entity.HasOne(d => d.UsuarioUsernameNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.Username)
                .HasConstraintName("productos_username_fkey");
        });


        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("usuario_pkey");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Cedula, "usuario_cedula_key").IsUnique();

            entity.HasIndex(e => e.Correo, "usuario_correo_key").IsUnique();

            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.Cedula)
                .HasMaxLength(10)
                .HasColumnName("cedula");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

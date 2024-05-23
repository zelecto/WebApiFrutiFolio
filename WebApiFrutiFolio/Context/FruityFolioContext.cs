using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebApiFrutiFolio.Models;

namespace WebApiFrutiFolio.Context
{
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
        public virtual DbSet<ClienteUsuario> ClienteUsuarios { get; set; }
        public virtual DbSet<TiendaVirtual> TiendasVirtuales { get; set; }
        public virtual DbSet<Pedido> Pedidos { get; set; }

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

            modelBuilder.Entity<ClienteUsuario>(entity =>
            {
                entity.HasKey(e => e.Username).HasName("cliente_usuario_pkey");

                entity.ToTable("clienteusuario");

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
                entity.Property(e => e.Ciudad)
                    .HasMaxLength(255)
                    .HasColumnName("ciudad");
                entity.Property(e => e.DireccionResidencia)
                    .HasMaxLength(255)
                    .HasColumnName("direccion_residencia");

                entity.HasMany(e => e.Pedidos)
                    .WithOne(p => p.ClienteUsuario)
                    .HasForeignKey(p => p.Username_Cliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pedido_cliente_usuario_fkey");
            });

            modelBuilder.Entity<TiendaVirtual>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("tienda_virtual_pkey");

                entity.ToTable("tiendavirtual");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(255)
                    .HasColumnName("nombre");
                entity.Property(e => e.Ciudad)
                    .HasMaxLength(255)
                    .HasColumnName("ciudad");
                entity.Property(e => e.Direccion)
                    .HasMaxLength(255)
                    .HasColumnName("direccion");

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.TiendaVirtual)
                    .HasForeignKey(d => d.Username)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tienda_virtual_usuario_fkey");
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pedido_pkey");

                entity.ToTable("pedido");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Estado)
                    .HasMaxLength(50)
                    .HasColumnName("estado");
                entity.Property(e => e.Id_Factura)
                    .HasColumnName("id_factura");
                entity.Property(e => e.Id_Tienda)
                    .HasColumnName("id_tienda");
                entity.Property(e => e.PrecioTransporte)
                    .HasPrecision(10, 2)
                    .HasColumnName("preciotransporte");
                entity.Property(e => e.Username_Cliente)
                    .HasMaxLength(50)
                    .HasColumnName("username_cliente");

                entity.HasOne(d => d.Factura)
                    .WithMany(p => p.Pedidos)
                    .HasForeignKey(d => d.Id_Factura)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pedido_factura_fkey");

                entity.HasOne(d => d.TiendaVirtual)
                    .WithMany(p => p.Pedidos)
                    .HasForeignKey(d => d.Id_Tienda)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pedido_tienda_virtual_fkey");

                entity.HasOne(d => d.ClienteUsuario)
                    .WithMany(p => p.Pedidos)
                    .HasForeignKey(d => d.Username_Cliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pedido_cliente_usuario_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace app.Server.Models;

public partial class EcodbContext : DbContext
{
    public EcodbContext()
    {
    }

    public EcodbContext(DbContextOptions<EcodbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Acceptance> Acceptances { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<HazardClass> HazardClasses { get; set; }

    public virtual DbSet<HazardousWaste> HazardousWastes { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<Point> Points { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ReceivingDiscount> ReceivingDiscounts { get; set; }

    public virtual DbSet<ReceivingProduct> ReceivingProducts { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=ecodb;user=root;password=4286Avetisova", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.37-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Acceptance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("acceptance");

            entity.HasIndex(e => e.HazardousWasteId, "IX_acceptance_hazardous_waste_id");

            entity.HasIndex(e => e.TransactionId, "IX_acceptance_transaction_id");

            entity.HasIndex(e => e.PointId, "acceptance_points_FK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasMaxLength(6)
                .HasColumnName("date");
            entity.Property(e => e.HazardousWasteId).HasColumnName("hazardous_waste_id");
            entity.Property(e => e.PointId)
                .HasDefaultValueSql("'1'")
                .HasColumnName("point_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            entity.HasOne(d => d.HazardousWaste).WithMany(p => p.Acceptances)
                .HasForeignKey(d => d.HazardousWasteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("acceptance_hazardous_waste_id_fk");

            entity.HasOne(d => d.Point).WithMany(p => p.Acceptances)
                .HasForeignKey(d => d.PointId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("acceptance_points_FK");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Acceptances)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("acceptance_transaction_id_fk");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("discounts");

            entity.HasIndex(e => e.PartnerId, "IX_discounts_partner_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bonuses).HasColumnName("bonuses");
            entity.Property(e => e.DateEnd)
                .HasMaxLength(6)
                .HasColumnName("date_end");
            entity.Property(e => e.DateStart)
                .HasMaxLength(6)
                .HasColumnName("date_start");
            entity.Property(e => e.PartnerId).HasColumnName("partner_id");
            entity.Property(e => e.Terms).HasColumnName("terms");

            entity.HasOne(d => d.Partner).WithMany(p => p.Discounts).HasForeignKey(d => d.PartnerId);
        });

        modelBuilder.Entity<Efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__efmigrationshistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<HazardClass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("hazard_class");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .HasColumnName("name");
        });

        modelBuilder.Entity<HazardousWaste>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("hazardous_waste");

            entity.HasIndex(e => e.HazardClassId, "IX_hazardous_waste_hazard_class_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bonuses).HasColumnName("bonuses");
            entity.Property(e => e.HazardClassId).HasColumnName("hazard_class_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.HazardClass).WithMany(p => p.HazardousWastes)
                .HasForeignKey(d => d.HazardClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("hazardous_waste_hazard_class_id_fk");
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("partners");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Point>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("points");

            entity.HasIndex(e => e.Name, "points_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.Name, "products_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bonus).HasColumnName("bonus");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ReceivingDiscount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("receiving_discounts");

            entity.HasIndex(e => e.DiscountId, "IX_receiving_discounts_discount_id");

            entity.HasIndex(e => e.TransactionId, "IX_receiving_discounts_transaction_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasMaxLength(6)
                .HasColumnName("date");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            entity.HasOne(d => d.Discount).WithMany(p => p.ReceivingDiscounts)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("receiving_discounts_discount_id_fk");

            entity.HasOne(d => d.Transaction).WithMany(p => p.ReceivingDiscounts)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("receiving_discounts_transaction_id_fk");
        });

        modelBuilder.Entity<ReceivingProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("receiving_products");

            entity.HasIndex(e => e.ProductId, "receiving_products_products_FK");

            entity.HasIndex(e => e.TransactionId, "receiving_products_transactions_FK");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ReceivingProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("receiving_products_products_FK");

            entity.HasOne(d => d.Transaction).WithMany(p => p.ReceivingProducts)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("receiving_products_transactions_FK");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transactions");

            entity.HasIndex(e => e.TypeId, "IX_transactions_type_id");

            entity.HasIndex(e => e.UserId, "IX_transactions_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BonusesEnd).HasColumnName("bonuses_end");
            entity.Property(e => e.BonusesStart).HasColumnName("bonuses_start");
            entity.Property(e => e.Date)
                .HasMaxLength(6)
                .HasColumnName("date");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Type).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactions_type_id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactions_user_id_fk");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("transaction_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.RoleId, "IX_users_role_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bonuses).HasColumnName("bonuses");
            entity.Property(e => e.EmailHash)
                .HasColumnType("text")
                .HasColumnName("email_hash");
            entity.Property(e => e.Encrypt)
                .HasColumnType("text")
                .HasColumnName("encrypt");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_role_id_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

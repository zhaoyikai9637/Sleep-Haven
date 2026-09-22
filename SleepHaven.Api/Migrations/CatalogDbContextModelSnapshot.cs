using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using SleepHaven.Api.Data;

#nullable disable

namespace SleepHaven.Api.Migrations;

[DbContext(typeof(CatalogDbContext))]
partial class CatalogDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.4")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity("SleepHaven.Product", product =>
        {
            product.Property<string>("Id")
                .HasMaxLength(64)
                .HasColumnType("character varying(64)");
            product.Property<int>("Currency").HasColumnType("integer");
            product.Property<string>("Description").IsRequired().HasColumnType("text");
            product.Property<string>("LandscapeUrl").IsRequired().HasMaxLength(255).HasColumnType("character varying(255)");
            product.Property<int>("Material").HasColumnType("integer");
            product.Property<string>("Name").IsRequired().HasMaxLength(255).HasColumnType("character varying(255)");
            product.Property<decimal>("Price").HasPrecision(12, 2).HasColumnType("numeric(12,2)");
            product.Property<int>("ProductType").HasColumnType("integer");
            product.Property<int>("Season").HasColumnType("integer");
            product.Property<string>("ThumbnailUrl").IsRequired().HasMaxLength(255).HasColumnType("character varying(255)");
            product.HasKey("Id");
            product.ToTable("products");
        });

        modelBuilder.Entity("SleepHaven.Api.Data.Favorite", favorite =>
        {
            favorite.Property<string>("ClientId").HasMaxLength(128).HasColumnType("character varying(128)");
            favorite.Property<string>("ProductId").HasMaxLength(64).HasColumnType("character varying(64)");
            favorite.Property<DateTimeOffset>("CreatedAt").HasColumnType("timestamp with time zone");
            favorite.HasKey("ClientId", "ProductId");
            favorite.HasIndex("ProductId");
            favorite.ToTable("favorites");
        });

        modelBuilder.Entity("SleepHaven.Api.Data.Favorite", favorite =>
        {
            favorite.HasOne("SleepHaven.Product", "Product")
                .WithMany()
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            favorite.Navigation("Product");
        });
#pragma warning restore 612, 618
    }
}

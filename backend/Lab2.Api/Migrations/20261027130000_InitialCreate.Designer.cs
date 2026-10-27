using System;
using Lab2.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab2.Api.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261027130000_InitialCreate")]
public partial class InitialCreate
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {

        modelBuilder.HasAnnotation("ProductVersion", "8.0.32");

        modelBuilder.Entity("Lab2.Api.Models.Product", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                .HasAnnotation("Sqlite:Autoincrement", true);

            b.Property<DateTime>("CreatedAt").HasColumnType("TEXT");
            b.Property<string>("Description").HasMaxLength(500).HasColumnType("TEXT");
            b.Property<string>("Name").IsRequired().HasMaxLength(120).HasColumnType("TEXT");
            b.Property<long>("Price").HasColumnType("INTEGER");
            b.Property<int>("Quantity").HasColumnType("INTEGER");

            b.HasKey("Id");
            b.ToTable("Products");
        });

    }
}

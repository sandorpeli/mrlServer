﻿// <auto-generated />
using System;
using MRLserver.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MRLserver.Migrations
{
    [DbContext(typeof(MRLservContext))]
    partial class MRLservContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MRLserver.Models.MRLmodel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("UID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("karbantarto")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("kovetkezoKarbantartas")
                        .HasColumnType("datetime2");

                    b.Property<string>("telepitesHelye")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("telepitesIdeje")
                        .HasColumnType("datetime2");

                    b.Property<string>("telepitesPozicioja")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("telepitestVegezte")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("utolsoKapcsolataLifttel")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("utolsoKarbantartasIdeje")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("MRLmodel");
                });

            modelBuilder.Entity("MRLserver.Models.MRLtelemetryModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("DoorStateA")
                        .HasColumnType("int");

                    b.Property<int>("DoorStateB")
                        .HasColumnType("int");

                    b.Property<int>("ElevatorState")
                        .HasColumnType("int");

                    b.Property<string>("Errors")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Travel1")
                        .HasColumnType("int");

                    b.Property<int>("Travel2")
                        .HasColumnType("int");

                    b.Property<string>("UID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VVVFErrors")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("utolsoKapcsolataLifttel")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.ToTable("MRLtelemetryModel");
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductivityKeeperWeb.Data;

namespace ProductivityKeeperWeb.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20211222001237_Change2")]
    partial class Change2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ColorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateOfCreation")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ColorId");

                    b.HasIndex("UnitId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Color", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("A")
                        .HasColumnType("int");

                    b.Property<int>("B")
                        .HasColumnType("int");

                    b.Property<int>("G")
                        .HasColumnType("int");

                    b.Property<int>("R")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Color");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Subcategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int?>("ColorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateOfCreation")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ColorId");

                    b.ToTable("Subcategory");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateOfCreation")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DoneDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsChecked")
                        .HasColumnType("bit");

                    b.Property<int?>("SubcategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SubcategoryId");

                    b.ToTable("Task");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.User", b =>
                {
                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("HashPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<int>("UnitId")
                        .HasColumnType("int");

                    b.Property<int?>("UserSettingsId")
                        .HasColumnType("int");

                    b.HasKey("Email");

                    b.HasIndex("UserSettingsId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.UserSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AppTheme")
                        .HasColumnType("int");

                    b.Property<bool>("AutoMoveTasksThatExpired")
                        .HasColumnType("bit");

                    b.Property<int>("GeneratingColorsWay")
                        .HasColumnType("int");

                    b.Property<bool>("IsSynchronizationOn")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Category", b =>
                {
                    b.HasOne("ProductivityKeeperWeb.Models.TaskRelated.Color", "Color")
                        .WithMany()
                        .HasForeignKey("ColorId");

                    b.HasOne("ProductivityKeeperWeb.Models.TaskRelated.Unit", null)
                        .WithMany("Categories")
                        .HasForeignKey("UnitId");

                    b.Navigation("Color");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Subcategory", b =>
                {
                    b.HasOne("ProductivityKeeperWeb.Models.TaskRelated.Category", null)
                        .WithMany("Subcategories")
                        .HasForeignKey("CategoryId");

                    b.HasOne("ProductivityKeeperWeb.Models.TaskRelated.Color", "Color")
                        .WithMany()
                        .HasForeignKey("ColorId");

                    b.Navigation("Color");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Task", b =>
                {
                    b.HasOne("ProductivityKeeperWeb.Models.TaskRelated.Subcategory", null)
                        .WithMany("Tasks")
                        .HasForeignKey("SubcategoryId");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.User", b =>
                {
                    b.HasOne("ProductivityKeeperWeb.Models.UserSettings", "UserSettings")
                        .WithMany()
                        .HasForeignKey("UserSettingsId");

                    b.Navigation("UserSettings");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Category", b =>
                {
                    b.Navigation("Subcategories");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Subcategory", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Unit", b =>
                {
                    b.Navigation("Categories");
                });
#pragma warning restore 612, 618
        }
    }
}

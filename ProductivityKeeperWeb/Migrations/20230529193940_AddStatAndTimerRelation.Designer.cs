﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductivityKeeperWeb.Data;

#nullable disable

namespace ProductivityKeeperWeb.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20230529193940_AddStatAndTimerRelation")]
    partial class AddStatAndTimerRelation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.TaskRelated.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ColorHex")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfCreation")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<int>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UnitId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.TaskRelated.Subcategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("ColorHex")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfCreation")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Subcategories");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.TaskRelated.SubcategoryTask", b =>
                {
                    b.Property<int>("SubcategoryId")
                        .HasColumnType("int");

                    b.Property<int>("TaskItemId")
                        .HasColumnType("int");

                    b.HasKey("SubcategoryId", "TaskItemId");

                    b.HasIndex("TaskItemId");

                    b.ToTable("SubcategoryTask", (string)null);
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.TaskRelated.TaskItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateOfCreation")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DoneDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("GoalRepeatCount")
                        .HasColumnType("int");

                    b.Property<double?>("HabbitIntervalInHours")
                        .HasColumnType("float");

                    b.Property<bool>("IsChecked")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRepeatable")
                        .HasColumnType("bit");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TimesToRepeat")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.TaskRelated.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("StatisticId")
                        .HasColumnType("int");

                    b.Property<int>("TimerId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.Timer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Format")
                        .HasColumnType("int");

                    b.Property<long>("Goal")
                        .HasColumnType("bigint");

                    b.Property<string>("Label")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Ticked")
                        .HasColumnType("bigint");

                    b.Property<int>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UnitId")
                        .IsUnique();

                    b.ToTable("Timers");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.User", b =>
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

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.UserSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

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

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.UserStatistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CountOfDoneToday")
                        .HasColumnType("int");

                    b.Property<int>("CountOfDoneTotal")
                        .HasColumnType("int");

                    b.Property<int>("CountOfExpiredTotal")
                        .HasColumnType("int");

                    b.Property<float>("PercentOfDoneToday")
                        .HasColumnType("real");

                    b.Property<float>("PercentOfDoneTotal")
                        .HasColumnType("real");

                    b.Property<int>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UnitId")
                        .IsUnique();

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.TaskRelated.Category", b =>
                {
                    b.HasOne("ProductivityKeeperWeb.Domain.Models.TaskRelated.Unit", null)
                        .WithMany("Categories")
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.TaskRelated.Subcategory", b =>
                {
                    b.HasOne("ProductivityKeeperWeb.Domain.Models.TaskRelated.Category", "Category")
                        .WithMany("Subcategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.TaskRelated.SubcategoryTask", b =>
                {
                    b.HasOne("ProductivityKeeperWeb.Domain.Models.TaskRelated.Subcategory", "Subcategory")
                        .WithMany()
                        .HasForeignKey("SubcategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProductivityKeeperWeb.Domain.Models.TaskRelated.TaskItem", "TaskItem")
                        .WithMany()
                        .HasForeignKey("TaskItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subcategory");

                    b.Navigation("TaskItem");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.TaskRelated.Unit", b =>
                {
                    b.OwnsMany("ProductivityKeeperWeb.Domain.Models.TaskRelated.ArchivedTask", "TaskArchive", b1 =>
                        {
                            b1.Property<int>("UnitId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<DateTime?>("Deadline")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime?>("DoneDate")
                                .HasColumnType("datetime2");

                            b1.Property<int>("Status")
                                .HasColumnType("int");

                            b1.HasKey("UnitId", "Id");

                            b1.ToTable("ArchivedTask");

                            b1.WithOwner()
                                .HasForeignKey("UnitId");
                        });

                    b.Navigation("TaskArchive");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.Timer", b =>
                {
                    b.HasOne("ProductivityKeeperWeb.Domain.Models.TaskRelated.Unit", "Unit")
                        .WithOne("Timer")
                        .HasForeignKey("ProductivityKeeperWeb.Domain.Models.Timer", "UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.User", b =>
                {
                    b.HasOne("ProductivityKeeperWeb.Domain.Models.UserSettings", "UserSettings")
                        .WithMany()
                        .HasForeignKey("UserSettingsId");

                    b.Navigation("UserSettings");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.UserStatistic", b =>
                {
                    b.HasOne("ProductivityKeeperWeb.Domain.Models.TaskRelated.Unit", "Unit")
                        .WithOne("Statistic")
                        .HasForeignKey("ProductivityKeeperWeb.Domain.Models.UserStatistic", "UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("ProductivityKeeperWeb.Domain.Models.DonePerDay", "PerDayStatistic", b1 =>
                        {
                            b1.Property<int>("UserStatisticId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<int>("CountOfDone")
                                .HasColumnType("int");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("datetime2");

                            b1.HasKey("UserStatisticId", "Id");

                            b1.ToTable("DonePerDay");

                            b1.WithOwner()
                                .HasForeignKey("UserStatisticId");
                        });

                    b.Navigation("PerDayStatistic");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.TaskRelated.Category", b =>
                {
                    b.Navigation("Subcategories");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Domain.Models.TaskRelated.Unit", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("Statistic");

                    b.Navigation("Timer");
                });
#pragma warning restore 612, 618
        }
    }
}

// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductivityKeeperWeb.Data;

namespace ProductivityKeeperWeb.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

            modelBuilder.Entity("ProductivityKeeperWeb.Models.TaskRelated.Unit", b =>
                {
                    b.OwnsMany("ProductivityKeeperWeb.Models.TaskRelated.ArchivedTask", "TaskArchive", b1 =>
                        {
                            b1.Property<int>("UnitId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.OwnsMany("ProductivityKeeperWeb.Models.TaskRelated.Category", "Categories", b1 =>
                        {
                            b1.Property<int>("UnitId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<DateTime>("DateOfCreation")
                                .HasColumnType("datetime2");

                            b1.Property<bool>("IsVisible")
                                .HasColumnType("bit");

                            b1.Property<string>("Name")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("Position")
                                .HasColumnType("int");

                            b1.HasKey("UnitId", "Id");

                            b1.ToTable("Category");

                            b1.WithOwner()
                                .HasForeignKey("UnitId");

                            b1.OwnsOne("ProductivityKeeperWeb.Models.TaskRelated.Color", "Color", b2 =>
                                {
                                    b2.Property<int>("CategoryUnitId")
                                        .HasColumnType("int");

                                    b2.Property<int>("CategoryId")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int")
                                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                                    b2.Property<int>("A")
                                        .HasColumnType("int");

                                    b2.Property<int>("B")
                                        .HasColumnType("int");

                                    b2.Property<int>("G")
                                        .HasColumnType("int");

                                    b2.Property<int>("Id")
                                        .HasColumnType("int");

                                    b2.Property<int>("R")
                                        .HasColumnType("int");

                                    b2.HasKey("CategoryUnitId", "CategoryId");

                                    b2.ToTable("Category");

                                    b2.WithOwner()
                                        .HasForeignKey("CategoryUnitId", "CategoryId");
                                });

                            b1.OwnsMany("ProductivityKeeperWeb.Models.TaskRelated.Subcategory", "Subcategories", b2 =>
                                {
                                    b2.Property<int>("CategoryUnitId")
                                        .HasColumnType("int");

                                    b2.Property<int>("CategoryId")
                                        .HasColumnType("int");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int")
                                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                                    b2.Property<DateTime>("DateOfCreation")
                                        .HasColumnType("datetime2");

                                    b2.Property<string>("Name")
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<int>("Position")
                                        .HasColumnType("int");

                                    b2.HasKey("CategoryUnitId", "CategoryId", "Id");

                                    b2.ToTable("Subcategory");

                                    b2.WithOwner()
                                        .HasForeignKey("CategoryUnitId", "CategoryId");

                                    b2.OwnsOne("ProductivityKeeperWeb.Models.TaskRelated.Color", "Color", b3 =>
                                        {
                                            b3.Property<int>("SubcategoryCategoryUnitId")
                                                .HasColumnType("int");

                                            b3.Property<int>("SubcategoryCategoryId")
                                                .HasColumnType("int");

                                            b3.Property<int>("SubcategoryId")
                                                .ValueGeneratedOnAdd()
                                                .HasColumnType("int")
                                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                                            b3.Property<int>("A")
                                                .HasColumnType("int");

                                            b3.Property<int>("B")
                                                .HasColumnType("int");

                                            b3.Property<int>("G")
                                                .HasColumnType("int");

                                            b3.Property<int>("Id")
                                                .HasColumnType("int");

                                            b3.Property<int>("R")
                                                .HasColumnType("int");

                                            b3.HasKey("SubcategoryCategoryUnitId", "SubcategoryCategoryId", "SubcategoryId");

                                            b3.ToTable("Subcategory");

                                            b3.WithOwner()
                                                .HasForeignKey("SubcategoryCategoryUnitId", "SubcategoryCategoryId", "SubcategoryId");
                                        });

                                    b2.OwnsMany("ProductivityKeeperWeb.Models.TaskRelated.Task", "Tasks", b3 =>
                                        {
                                            b3.Property<int>("SubcategoryCategoryUnitId")
                                                .HasColumnType("int");

                                            b3.Property<int>("SubcategoryCategoryId")
                                                .HasColumnType("int");

                                            b3.Property<int>("SubcategoryId")
                                                .HasColumnType("int");

                                            b3.Property<int>("Id")
                                                .ValueGeneratedOnAdd()
                                                .HasColumnType("int")
                                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                                            b3.Property<DateTime>("DateOfCreation")
                                                .HasColumnType("datetime2");

                                            b3.Property<DateTime?>("Deadline")
                                                .HasColumnType("datetime2");

                                            b3.Property<DateTime?>("DoneDate")
                                                .HasColumnType("datetime2");

                                            b3.Property<int?>("GoalRepeatCount")
                                                .HasColumnType("int");

                                            b3.Property<double?>("HabbitIntervalInHours")
                                                .HasColumnType("float");

                                            b3.Property<bool>("IsChecked")
                                                .HasColumnType("bit");

                                            b3.Property<bool>("IsRepeatable")
                                                .HasColumnType("bit");

                                            b3.Property<int?>("RelationId")
                                                .HasColumnType("int");

                                            b3.Property<string>("Text")
                                                .HasColumnType("nvarchar(max)");

                                            b3.Property<int?>("TimesToRepeat")
                                                .HasColumnType("int");

                                            b3.HasKey("SubcategoryCategoryUnitId", "SubcategoryCategoryId", "SubcategoryId", "Id");

                                            b3.ToTable("Task");

                                            b3.WithOwner()
                                                .HasForeignKey("SubcategoryCategoryUnitId", "SubcategoryCategoryId", "SubcategoryId");
                                        });

                                    b2.Navigation("Color");

                                    b2.Navigation("Tasks");
                                });

                            b1.Navigation("Color");

                            b1.Navigation("Subcategories");
                        });

                    b.OwnsMany("ProductivityKeeperWeb.Models.TaskRelated.TaskToManySubcategories", "TaskToManySubcategories", b1 =>
                        {
                            b1.Property<int>("UnitId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.HasKey("UnitId", "Id");

                            b1.ToTable("TaskToManySubcategories");

                            b1.WithOwner()
                                .HasForeignKey("UnitId");

                            b1.OwnsMany("ProductivityKeeperWeb.Models.TaskRelated.TaskSubcategory", "TaskSubcategories", b2 =>
                                {
                                    b2.Property<int>("TaskToManySubcategoriesUnitId")
                                        .HasColumnType("int");

                                    b2.Property<int>("TaskToManySubcategoriesId")
                                        .HasColumnType("int");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int")
                                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                                    b2.Property<int>("CategoryId")
                                        .HasColumnType("int");

                                    b2.Property<int>("SubcategoryId")
                                        .HasColumnType("int");

                                    b2.Property<int>("TaskId")
                                        .HasColumnType("int");

                                    b2.HasKey("TaskToManySubcategoriesUnitId", "TaskToManySubcategoriesId", "Id");

                                    b2.ToTable("TaskSubcategory");

                                    b2.WithOwner()
                                        .HasForeignKey("TaskToManySubcategoriesUnitId", "TaskToManySubcategoriesId");
                                });

                            b1.Navigation("TaskSubcategories");
                        });

                    b.OwnsOne("ProductivityKeeperWeb.Models.Timer", "Timer", b1 =>
                        {
                            b1.Property<int>("UnitId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<int>("Format")
                                .HasColumnType("int");

                            b1.Property<long>("Goal")
                                .HasColumnType("bigint");

                            b1.Property<int>("Id")
                                .HasColumnType("int");

                            b1.Property<string>("Label")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<long>("Ticked")
                                .HasColumnType("bigint");

                            b1.HasKey("UnitId");

                            b1.ToTable("Units");

                            b1.WithOwner()
                                .HasForeignKey("UnitId");
                        });

                    b.OwnsOne("ProductivityKeeperWeb.Models.UserStatistic", "Statistic", b1 =>
                        {
                            b1.Property<int>("UnitId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<int>("CountOfDoneToday")
                                .HasColumnType("int");

                            b1.Property<int>("CountOfDoneTotal")
                                .HasColumnType("int");

                            b1.Property<int>("CountOfExpiredTotal")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .HasColumnType("int");

                            b1.Property<float>("PercentOfDoneToday")
                                .HasColumnType("real");

                            b1.Property<float>("PercentOfDoneTotal")
                                .HasColumnType("real");

                            b1.HasKey("UnitId");

                            b1.ToTable("Units");

                            b1.WithOwner()
                                .HasForeignKey("UnitId");

                            b1.OwnsMany("ProductivityKeeperWeb.Models.DonePerDay", "PerDayStatistic", b2 =>
                                {
                                    b2.Property<int>("UserStatisticUnitId")
                                        .HasColumnType("int");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int")
                                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                                    b2.Property<int>("CountOfDone")
                                        .HasColumnType("int");

                                    b2.Property<DateTime>("Date")
                                        .HasColumnType("datetime2");

                                    b2.HasKey("UserStatisticUnitId", "Id");

                                    b2.ToTable("DonePerDay");

                                    b2.WithOwner()
                                        .HasForeignKey("UserStatisticUnitId");
                                });

                            b1.Navigation("PerDayStatistic");
                        });

                    b.Navigation("Categories");

                    b.Navigation("Statistic");

                    b.Navigation("TaskArchive");

                    b.Navigation("TaskToManySubcategories");

                    b.Navigation("Timer");
                });

            modelBuilder.Entity("ProductivityKeeperWeb.Models.User", b =>
                {
                    b.HasOne("ProductivityKeeperWeb.Models.UserSettings", "UserSettings")
                        .WithMany()
                        .HasForeignKey("UserSettingsId");

                    b.Navigation("UserSettings");
                });
#pragma warning restore 612, 618
        }
    }
}

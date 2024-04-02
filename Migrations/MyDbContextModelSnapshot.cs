﻿// <auto-generated />
using System;
using IMDBLib.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IMDBLib.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GenreTitle", b =>
                {
                    b.Property<int>("GenresId")
                        .HasColumnType("int");

                    b.Property<string>("TitlesTconst")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GenresId", "TitlesTconst");

                    b.HasIndex("TitlesTconst");

                    b.ToTable("GenreTitle");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("GenreName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.Title", b =>
                {
                    b.Property<string>("Tconst")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("EndYear")
                        .HasColumnType("int");

                    b.Property<bool>("IsAdult")
                        .HasColumnType("bit");

                    b.Property<string>("OriginalTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrimaryTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RuntimeMinutes")
                        .HasColumnType("int");

                    b.Property<int>("StartYear")
                        .HasColumnType("int");

                    b.Property<int>("TitleTypeId")
                        .HasColumnType("int");

                    b.HasKey("Tconst");

                    b.HasIndex("TitleTypeId");

                    b.ToTable("Titles");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.TitleType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TitleTypes");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.Title_Genre", b =>
                {
                    b.Property<string>("TitleTconst")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("TitleTconst", "GenreId");

                    b.HasIndex("GenreId");

                    b.ToTable("Title_Genres");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Crew", b =>
                {
                    b.Property<string>("Nconst")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("BirthYear")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeathYear")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrimaryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Nconst");

                    b.ToTable("Crews");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Crew_Profession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CrewNconst")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("JobId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CrewNconst");

                    b.HasIndex("JobId");

                    b.ToTable("Crew_Professions");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Directors", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DirectorNconst")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Title_CrewId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DirectorNconst");

                    b.HasIndex("Title_CrewId");

                    b.ToTable("Directors");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Job", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Known_For_Titles", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CrewNconst")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TitleTconst")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CrewNconst");

                    b.HasIndex("TitleTconst");

                    b.ToTable("Know_For_Titles");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Title_Crew", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("JobId")
                        .HasColumnType("int");

                    b.Property<string>("TitleTconst")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("JobId");

                    b.HasIndex("TitleTconst")
                        .IsUnique();

                    b.ToTable("Title_Crews");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Writers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Title_CrewId")
                        .HasColumnType("int");

                    b.Property<string>("WriterNconst")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Title_CrewId");

                    b.HasIndex("WriterNconst");

                    b.ToTable("Writers");
                });

            modelBuilder.Entity("IMDBLib.Models.Views.MovieView", b =>
                {
                    b.Property<int>("ID")
                        .HasColumnType("int");

                    b.Property<int>("EndYear")
                        .HasColumnType("int");

                    b.Property<string>("GenreNames")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdult")
                        .HasColumnType("bit");

                    b.Property<string>("OriginalTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrimaryTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RuntimeMinutes")
                        .HasColumnType("int");

                    b.Property<int>("StartYear")
                        .HasColumnType("int");

                    b.Property<string>("Tconst")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable((string)null);

                    b.ToView("MovieView", (string)null);
                });

            modelBuilder.Entity("GenreTitle", b =>
                {
                    b.HasOne("IMDBLib.Models.Movie.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IMDBLib.Models.Movie.Title", null)
                        .WithMany()
                        .HasForeignKey("TitlesTconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.Title", b =>
                {
                    b.HasOne("IMDBLib.Models.Movie.TitleType", "TitleType")
                        .WithMany("Titles")
                        .HasForeignKey("TitleTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TitleType");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.Title_Genre", b =>
                {
                    b.HasOne("IMDBLib.Models.Movie.Genre", "Genre")
                        .WithMany("Title_Genres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IMDBLib.Models.Movie.Title", "Title")
                        .WithMany("Title_Genres")
                        .HasForeignKey("TitleTconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genre");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Crew_Profession", b =>
                {
                    b.HasOne("IMDBLib.Models.People.Crew", "Crew")
                        .WithMany("Crew_Professions")
                        .HasForeignKey("CrewNconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IMDBLib.Models.People.Job", "Job")
                        .WithMany("Crew_Profession")
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Crew");

                    b.Navigation("Job");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Directors", b =>
                {
                    b.HasOne("IMDBLib.Models.People.Crew", "Crew")
                        .WithMany("Directors")
                        .HasForeignKey("DirectorNconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IMDBLib.Models.People.Title_Crew", "Title_Crew")
                        .WithMany("Directors")
                        .HasForeignKey("Title_CrewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Crew");

                    b.Navigation("Title_Crew");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Known_For_Titles", b =>
                {
                    b.HasOne("IMDBLib.Models.People.Crew", "Crew")
                        .WithMany("Know_For_Titles")
                        .HasForeignKey("CrewNconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IMDBLib.Models.Movie.Title", "Title")
                        .WithMany("Know_For_Titles")
                        .HasForeignKey("TitleTconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Crew");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Title_Crew", b =>
                {
                    b.HasOne("IMDBLib.Models.People.Job", null)
                        .WithMany("Title_Crews")
                        .HasForeignKey("JobId");

                    b.HasOne("IMDBLib.Models.Movie.Title", "Title")
                        .WithOne("Title_Crew")
                        .HasForeignKey("IMDBLib.Models.People.Title_Crew", "TitleTconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Title");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Writers", b =>
                {
                    b.HasOne("IMDBLib.Models.People.Title_Crew", "Title_Crew")
                        .WithMany("Writers")
                        .HasForeignKey("Title_CrewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IMDBLib.Models.People.Crew", "Crew")
                        .WithMany("Writers")
                        .HasForeignKey("WriterNconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Crew");

                    b.Navigation("Title_Crew");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.Genre", b =>
                {
                    b.Navigation("Title_Genres");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.Title", b =>
                {
                    b.Navigation("Know_For_Titles");

                    b.Navigation("Title_Crew")
                        .IsRequired();

                    b.Navigation("Title_Genres");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.TitleType", b =>
                {
                    b.Navigation("Titles");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Crew", b =>
                {
                    b.Navigation("Crew_Professions");

                    b.Navigation("Directors");

                    b.Navigation("Know_For_Titles");

                    b.Navigation("Writers");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Job", b =>
                {
                    b.Navigation("Crew_Profession");

                    b.Navigation("Title_Crews");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Title_Crew", b =>
                {
                    b.Navigation("Directors");

                    b.Navigation("Writers");
                });
#pragma warning restore 612, 618
        }
    }
}

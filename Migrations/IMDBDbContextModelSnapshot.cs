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
    [DbContext(typeof(IMDBDbContext))]
    partial class IMDBDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("IMDBLib.Models.Movie.Genre", b =>
                {
                    b.Property<int>("GenreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GenreId"));

                    b.Property<string>("GenreName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GenreId");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.Title", b =>
                {
                    b.Property<string>("Tconst")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("EndYear")
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

                    b.Property<string>("TitleType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Tconst");

                    b.ToTable("Titles");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.TitleGenre", b =>
                {
                    b.Property<string>("Tconst")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.HasKey("Tconst", "GenreId");

                    b.HasIndex("GenreId");

                    b.ToTable("TitleGenres");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Person", b =>
                {
                    b.Property<string>("Nconst")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("BirthYear")
                        .HasColumnType("int");

                    b.Property<int?>("DeathYear")
                        .HasColumnType("int");

                    b.Property<string>("PrimaryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Nconst");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("IMDBLib.Models.People.PersonProfession", b =>
                {
                    b.Property<string>("Nconst")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ProfessionId")
                        .HasColumnType("int");

                    b.HasKey("Nconst", "ProfessionId");

                    b.HasIndex("ProfessionId");

                    b.ToTable("PersonProfessions");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Profession", b =>
                {
                    b.Property<int>("ProfessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProfessionId"));

                    b.Property<string>("ProfessionName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProfessionId");

                    b.ToTable("Professions");
                });

            modelBuilder.Entity("IMDBLib.Models.People.TitlePerson", b =>
                {
                    b.Property<string>("Tconst")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Nconst")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Tconst", "Nconst");

                    b.HasIndex("Nconst");

                    b.ToTable("TitlePersons");
                });

            modelBuilder.Entity("IMDBLib.Models.Views.MovieView", b =>
                {
                    b.Property<int?>("EndYear")
                        .HasColumnType("int");

                    b.Property<string>("GenreName")
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

                    b.Property<string>("TitleType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable((string)null);

                    b.ToView("MovieView", (string)null);
                });

            modelBuilder.Entity("IMDBLib.Models.Views.PersonView", b =>
                {
                    b.Property<int>("BirthYear")
                        .HasColumnType("int");

                    b.Property<int?>("DeathYear")
                        .HasColumnType("int");

                    b.Property<string>("Nconst")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrimaryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfessionNames")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable((string)null);

                    b.ToView("PersonView", (string)null);
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.TitleGenre", b =>
                {
                    b.HasOne("IMDBLib.Models.Movie.Genre", "Genre")
                        .WithMany("TitleGenres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IMDBLib.Models.Movie.Title", "Title")
                        .WithMany("TitleGenres")
                        .HasForeignKey("Tconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genre");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("IMDBLib.Models.People.PersonProfession", b =>
                {
                    b.HasOne("IMDBLib.Models.People.Person", "Person")
                        .WithMany("PersonProfessions")
                        .HasForeignKey("Nconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IMDBLib.Models.People.Profession", "Profession")
                        .WithMany("PersonProfessions")
                        .HasForeignKey("ProfessionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Person");

                    b.Navigation("Profession");
                });

            modelBuilder.Entity("IMDBLib.Models.People.TitlePerson", b =>
                {
                    b.HasOne("IMDBLib.Models.People.Person", "Person")
                        .WithMany("TitlePersons")
                        .HasForeignKey("Nconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IMDBLib.Models.Movie.Title", "Title")
                        .WithMany("TitlePersons")
                        .HasForeignKey("Tconst")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Person");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.Genre", b =>
                {
                    b.Navigation("TitleGenres");
                });

            modelBuilder.Entity("IMDBLib.Models.Movie.Title", b =>
                {
                    b.Navigation("TitleGenres");

                    b.Navigation("TitlePersons");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Person", b =>
                {
                    b.Navigation("PersonProfessions");

                    b.Navigation("TitlePersons");
                });

            modelBuilder.Entity("IMDBLib.Models.People.Profession", b =>
                {
                    b.Navigation("PersonProfessions");
                });
#pragma warning restore 612, 618
        }
    }
}

using IMDBLib.Models.Movie;
using IMDBLib.Models.People;
using IMDBLib.Models.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IMDBLib.DataBase
{
    public class MyDbContext : DbContext
    {
        public DbSet<Title> Titles { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<TitleType> TitleTypes { get; set; }
        public DbSet<Title_Genre> Title_Genres { get; set; }
        public DbSet<Crew> Crews { get; set; }
        public DbSet<Crew_Profession> Crew_Professions { get; set; }
        public DbSet<Directors> Directors { get; set; }
        public DbSet<Known_For_Titles> Know_For_Titles { get; set; }
        public DbSet<Writers> Writers { get; set; }
        public DbSet<Title_Crew> Title_Crews { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<MovieView> MovieViews { get; set; } // Add DbSet for MovieView
        public DbSet<PersonView> PersonViews { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IMDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define MovieView entity
            modelBuilder.Entity<MovieView>().ToView("MovieView");

            // Define PersonView entity
            modelBuilder.Entity<PersonView>().ToView("PersonView");

            // Define relationship between Title and Genre through Title_Genre
            modelBuilder.Entity<Title_Genre>()
                .HasKey(tg => new { tg.TitleTconst, tg.GenreId });

            // Define relationship between Title and Genre
            modelBuilder.Entity<Title_Genre>()
                .HasOne(tg => tg.Title)
                .WithMany(t => t.Title_Genres)
                .HasForeignKey(tg => tg.TitleTconst);

            // Define relationship between Genre and Title
            modelBuilder.Entity<Title_Genre>()
                .HasOne(tg => tg.Genre)
                .WithMany(g => g.Title_Genres)
                .HasForeignKey(tg => tg.GenreId);

            // Define relationship between Title and TitleType
            modelBuilder.Entity<Title>()
                .HasOne(t => t.TitleType)
                .WithMany(tt => tt.Titles)
                .HasForeignKey(t => t.TitleTypeId);

            // Define relationship between Title_crew and Writers
            modelBuilder.Entity<Writers>()
                .HasOne(w => w.Title_Crew)
                .WithMany(tc => tc.Writers)
                .HasForeignKey(w => w.Title_CrewId);

            // Define relationship between Title_crew and Directors
            modelBuilder.Entity<Directors>()
                .HasOne(w => w.Title_Crew)
                .WithMany(tc => tc.Directors)
                .HasForeignKey(w => w.Title_CrewId);

            // Define relationship between Crew and Writers
            modelBuilder.Entity<Crew>()
                .HasMany(c => c.Writers)
                .WithOne(w => w.Crew)
                .HasForeignKey(w => w.WriterNconst);

            // Define relationship between Crew and Directors
            modelBuilder.Entity<Crew>()
                .HasMany(c => c.Directors)
                .WithOne(d => d.Crew)
                .HasForeignKey(d => d.DirectorNconst);

            // Define relationship between Crew and Crew_Profession
            modelBuilder.Entity<Crew_Profession>()
                .HasOne(cp => cp.Crew)
                .WithMany(c => c.Crew_Professions)
                .HasForeignKey(cp => cp.CrewNconst);

            // Define relationship between Crew_Profession and Job
            modelBuilder.Entity<Crew_Profession>()
                .HasOne(cp => cp.Job)
                .WithMany(j => j.Crew_Profession)
                .HasForeignKey(cp => cp.JobId);

            // Define relationship between Know_For_Titles and Crew
            modelBuilder.Entity<Known_For_Titles>()
                .HasOne(kft => kft.Crew)
                .WithMany(c => c.Know_For_Titles)
                .HasForeignKey(kft => kft.CrewNconst);

            // Define relationship between Know_For_Titles and Title
            modelBuilder.Entity<Known_For_Titles>()
                .HasOne(kft => kft.Title)
                .WithMany(t => t.Know_For_Titles)
                .HasForeignKey(kft => kft.TitleTconst);

            // Define relationship between Title and Title_Crew
            modelBuilder.Entity<Title_Crew>()
                .HasOne(tc => tc.Title)
                .WithOne(t => t.Title_Crew)
                .HasForeignKey<Title_Crew>(tc => tc.TitleTconst);


        }

        public IQueryable<MovieView> GetMovieView()
        {
            return Set<MovieView>().FromSqlRaw(@"
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY dbo.Titles.Tconst) AS ID, 
                    dbo.Titles.Tconst, 
                    dbo.Titles.PrimaryTitle, 
                    dbo.Titles.OriginalTitle, 
                    dbo.Titles.IsAdult, 
                    dbo.Titles.StartYear, 
                    dbo.Titles.EndYear, 
                    dbo.Titles.RuntimeMinutes, 
                    dbo.TitleTypes.Type, 
                    STRING_AGG(dbo.Genres.GenreName, ', ') AS GenreNames 
                FROM 
                    dbo.Genres 
                    INNER JOIN dbo.Title_Genres ON dbo.Genres.Id = dbo.Title_Genres.GenreId 
                    INNER JOIN dbo.Titles ON dbo.Title_Genres.TitleTconst = dbo.Titles.Tconst 
                    INNER JOIN dbo.TitleTypes ON dbo.Titles.TitleTypeId = dbo.TitleTypes.Id 
                GROUP BY 
                    dbo.Titles.Tconst, 
                    dbo.Titles.PrimaryTitle, 
                    dbo.Titles.OriginalTitle, 
                    dbo.Titles.IsAdult, 
                    dbo.Titles.StartYear, 
                    dbo.Titles.EndYear, 
                    dbo.Titles.RuntimeMinutes, 
                    dbo.TitleTypes.Type
            ");
        }

    }
}

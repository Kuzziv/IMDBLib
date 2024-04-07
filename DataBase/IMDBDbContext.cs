using IMDBLib.Models.Movie;
using IMDBLib.Models.People;
using IMDBLib.Models.Views;
using Microsoft.EntityFrameworkCore;

namespace IMDBLib.DataBase
{
    public class IMDBDbContext : DbContext
    {
        // DbSet properties to represent database tables
        public DbSet<Title> Titles { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<TitleGenre> TitleGenres { get; set; }
        public DbSet<TitlePerson> TitlePersons { get; set; }
        public DbSet<PersonProfession> PersonProfessions { get; set; }

        // DbSet properties to represent database views
        public DbSet<MovieView> MovieViews { get; set; }
        public DbSet<PersonView> PersonViews { get; set; }

        // Configure database connection and provider
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IMDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False")
                .EnableSensitiveDataLogging(true);
        }

        // Configure entity relationships and primary keys
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite primary keys for junction tables
            modelBuilder.Entity<TitleGenre>().HasKey(tg => new { tg.Tconst, tg.GenreId });
            modelBuilder.Entity<TitlePerson>().HasKey(tp => new { tp.Tconst, tp.Nconst });
            modelBuilder.Entity<PersonProfession>().HasKey(pp => new { pp.Nconst, pp.ProfessionId });

            // Configure primary keys for other entities
            modelBuilder.Entity<Title>().HasKey(t => t.Tconst);
            modelBuilder.Entity<Person>().HasKey(p => p.Nconst);

            // Configure relationships between entities
            modelBuilder.Entity<Title>()
                .HasMany(t => t.TitleGenres)
                .WithOne(tg => tg.Title)
                .HasForeignKey(tg => tg.Tconst);

            modelBuilder.Entity<Genre>()
                .HasMany(g => g.TitleGenres)
                .WithOne(tg => tg.Genre)
                .HasForeignKey(tg => tg.GenreId);

            modelBuilder.Entity<Title>()
                .HasMany(t => t.TitlePersons)
                .WithOne(tp => tp.Title)
                .HasForeignKey(tp => tp.Tconst);

            modelBuilder.Entity<Person>()
                .HasMany(p => p.TitlePersons)
                .WithOne(tp => tp.Person)
                .HasForeignKey(tp => tp.Nconst);

            modelBuilder.Entity<Person>()
                .HasMany(p => p.PersonProfessions)
                .WithOne(pp => pp.Person)
                .HasForeignKey(pp => pp.Nconst);

            modelBuilder.Entity<Profession>()
                .HasMany(p => p.PersonProfessions)
                .WithOne(pp => pp.Profession)
                .HasForeignKey(pp => pp.ProfessionId);

            // Map views to entities
            modelBuilder.Entity<MovieView>().ToView("MovieView").HasNoKey();
            modelBuilder.Entity<PersonView>().ToView("PersonView").HasNoKey();
        }
    }
}

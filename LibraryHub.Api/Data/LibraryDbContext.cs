using LibraryHub.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryHub.Api.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<Loan> Loans => Set<Loan>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Isbn).IsRequired().HasMaxLength(20);
                entity.HasIndex(b => b.Isbn).IsUnique();
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.Property(m => m.FullName).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Email).IsRequired().HasMaxLength(150);
                entity.Property(m => m.Phone).HasMaxLength(20);
                entity.HasIndex(m => m.Email).IsUnique();
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.Property(l => l.LateFee).HasPrecision(10, 2);

                entity.HasOne(l => l.Book)
                    .WithMany(b => b.Loans)
                    .HasForeignKey(l => l.BookId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.Member)
                    .WithMany(m => m.Loans)
                    .HasForeignKey(l => l.MemberId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Clean Code", Author = "Robert C. Martin", Isbn = "9780132350884", PublishedYear = 2008, TotalCopies = 3, AvailableCopies = 2 },
                new Book { Id = 2, Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Isbn = "9780135957059", PublishedYear = 2019, TotalCopies = 2, AvailableCopies = 1 },
                new Book { Id = 3, Title = "Design Patterns", Author = "Erich Gamma", Isbn = "9780201633610", PublishedYear = 1994, TotalCopies = 2, AvailableCopies = 2 },
                new Book { Id = 4, Title = "Refactoring", Author = "Martin Fowler", Isbn = "9780134757599", PublishedYear = 2018, TotalCopies = 2, AvailableCopies = 2 },
                new Book { Id = 5, Title = "Introduction to Algorithms", Author = "Thomas H. Cormen", Isbn = "9780262046305", PublishedYear = 2022, TotalCopies = 1, AvailableCopies = 1 },
                new Book { Id = 6, Title = "C# in Depth", Author = "Jon Skeet", Isbn = "9781617294532", PublishedYear = 2019, TotalCopies = 3, AvailableCopies = 3 },
                new Book { Id = 7, Title = "Domain-Driven Design", Author = "Eric Evans", Isbn = "9780321125217", PublishedYear = 2003, TotalCopies = 1, AvailableCopies = 1 },
                new Book { Id = 8, Title = "Code Complete", Author = "Steve McConnell", Isbn = "9780735619678", PublishedYear = 2004, TotalCopies = 2, AvailableCopies = 2 },
                new Book { Id = 9, Title = "The Mythical Man-Month", Author = "Frederick P. Brooks", Isbn = "9780201835953", PublishedYear = 1995, TotalCopies = 1, AvailableCopies = 1 },
                new Book { Id = 10, Title = "Head First Design Patterns", Author = "Eric Freeman", Isbn = "9781492078005", PublishedYear = 2020, TotalCopies = 2, AvailableCopies = 2 }
            );

            modelBuilder.Entity<Member>().HasData(
                new Member { Id = 1, FullName = "Ali Raza", Email = "ali.raza@example.com", Phone = "03001234567", JoinedOn = new DateTime(2026, 1, 10), IsActive = true },
                new Member { Id = 2, FullName = "Sara Khan", Email = "sara.khan@example.com", Phone = "03111234567", JoinedOn = new DateTime(2026, 2, 15), IsActive = true },
                new Member { Id = 3, FullName = "Usman Tariq", Email = "usman.tariq@example.com", Phone = "03211234567", JoinedOn = new DateTime(2026, 3, 5), IsActive = true },
                new Member { Id = 4, FullName = "Ayesha Noor", Email = "ayesha.noor@example.com", Phone = null, JoinedOn = new DateTime(2026, 4, 20), IsActive = true },
                new Member { Id = 5, FullName = "Hamza Iqbal", Email = "hamza.iqbal@example.com", Phone = "03331234567", JoinedOn = new DateTime(2026, 5, 1), IsActive = false }
            );

            modelBuilder.Entity<Loan>().HasData(
                new Loan { Id = 1, BookId = 1, MemberId = 1, BorrowedOn = new DateTime(2026, 11, 1), DueOn = new DateTime(2026, 11, 15), ReturnedOn = null, LateFee = 0 },
                new Loan { Id = 2, BookId = 2, MemberId = 2, BorrowedOn = new DateTime(2026, 11, 20), DueOn = new DateTime(2026, 12, 4), ReturnedOn = null, LateFee = 0 }
            );
        }
    }
}
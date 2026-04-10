using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SnapMyBook.Domain.Entities;
using SnapMyBook.Infrastructure.Identity;

namespace SnapMyBook.Infrastructure.Persistence;

public class SnapMyBookDbContext : IdentityDbContext<ApplicationUser>
{
    public SnapMyBookDbContext(DbContextOptions<SnapMyBookDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Highlight> Highlights => Set<Highlight>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<HighlightTag> HighlightTags => Set<HighlightTag>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Book>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Author).HasMaxLength(160);
            entity.Property(x => x.Isbn).HasMaxLength(32);
            entity.Property(x => x.CoverUrl).HasMaxLength(500);
            entity.Property(x => x.UserId).HasMaxLength(450).IsRequired();
            entity.HasMany(x => x.Highlights)
                .WithOne(x => x.Book)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Highlight>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RawText).HasMaxLength(8000);
            entity.Property(x => x.FinalText).HasMaxLength(8000).IsRequired();
            entity.Property(x => x.Chapter).HasMaxLength(160);
            entity.Property(x => x.ImagePath).HasMaxLength(500);
            entity.Property(x => x.Language).HasMaxLength(12).IsRequired();
        });

        builder.Entity<Tag>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(80).IsRequired();
            entity.Property(x => x.NormalizedName).HasMaxLength(80).IsRequired();
            entity.HasIndex(x => x.NormalizedName).IsUnique();
        });

        builder.Entity<HighlightTag>(entity =>
        {
            entity.HasKey(x => new { x.HighlightId, x.TagId });
            entity.HasOne(x => x.Highlight)
                .WithMany("HighlightTags")
                .HasForeignKey(x => x.HighlightId);
            entity.HasOne(x => x.Tag)
                .WithMany()
                .HasForeignKey(x => x.TagId);
        });
    }
}

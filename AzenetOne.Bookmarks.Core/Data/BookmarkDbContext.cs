using AzenetOne.Bookmarks.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzenetOne.Bookmarks.Core.Data;

public class BookmarkDbContext(DbContextOptions<BookmarkDbContext> options) : DbContext(options)
{
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var bookmark = modelBuilder.Entity<Bookmark>();

        bookmark.HasKey(x => x.Id);

        bookmark.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        bookmark.Property(x => x.Url)
            .HasMaxLength(2048)
            .IsRequired();

        bookmark.Property(x => x.Description)
            .HasMaxLength(2000);

        bookmark.Property(x => x.Tags)
            .HasMaxLength(1000);

        bookmark.HasIndex(x => x.Url)
            .IsUnique();

        bookmark.ToTable(t => t.HasCheckConstraint(
            "CK_Bookmark_Url_Format",
            "[Url] LIKE 'http://%' OR [Url] LIKE 'https://%'"));
    }
}

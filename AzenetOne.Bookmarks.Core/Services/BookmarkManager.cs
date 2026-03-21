using AzenetOne.Bookmarks.Core.Data;
using AzenetOne.Bookmarks.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzenetOne.Bookmarks.Core.Services;

public class BookmarkManager(BookmarkDbContext dbContext) : IBookmarkManager
{
    public async Task<Bookmark> CreateAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
    {
        ValidateBookmark(bookmark);
        bookmark.Normalize();

        dbContext.Bookmarks.Add(bookmark);
        await dbContext.SaveChangesAsync(cancellationToken);

        return bookmark;
    }

    public async Task<IReadOnlyList<Bookmark>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Bookmarks
            .AsNoTracking()
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<Bookmark> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var bookmark = await dbContext.Bookmarks
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return bookmark ?? throw new KeyNotFoundException($"Bookmark with id {id} was not found.");
    }

    public async Task<Bookmark> UpdateAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
    {
        ValidateBookmark(bookmark);

        var existing = await dbContext.Bookmarks
            .FirstOrDefaultAsync(x => x.Id == bookmark.Id, cancellationToken);

        if (existing is null)
        {
            throw new KeyNotFoundException($"Bookmark with id {bookmark.Id} was not found.");
        }

        existing.Title = bookmark.Title;
        existing.Url = bookmark.Url;
        existing.Description = bookmark.Description;
        existing.Tags = bookmark.Tags;
        existing.Normalize();

        await dbContext.SaveChangesAsync(cancellationToken);

        return existing;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Bookmarks
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (existing is null)
        {
            throw new KeyNotFoundException($"Bookmark with id {id} was not found.");
        }

        dbContext.Bookmarks.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateBookmark(Bookmark bookmark)
    {
        if (bookmark is null)
        {
            throw new ArgumentNullException(nameof(bookmark));
        }

        if (string.IsNullOrWhiteSpace(bookmark.Title))
        {
            throw new ArgumentException("Title is required.", nameof(bookmark));
        }

        if (string.IsNullOrWhiteSpace(bookmark.Url))
        {
            throw new ArgumentException("Url is required.", nameof(bookmark));
        }

        if (!Uri.TryCreate(bookmark.Url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Url must be a valid http or https URL.", nameof(bookmark));
        }
    }
}

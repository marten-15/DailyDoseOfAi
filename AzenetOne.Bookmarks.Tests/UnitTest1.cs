using AzenetOne.Bookmarks.Core.Data;
using AzenetOne.Bookmarks.Core.Entities;
using AzenetOne.Bookmarks.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace AzenetOne.Bookmarks.Tests;

public class BookmarkManagerTests
{
    [Fact]
    public async Task CreateAndListAsync_PersistsBookmark()
    {
        await using var dbContext = CreateDbContext();
        var manager = new BookmarkManager(dbContext);

        await manager.CreateAsync(new Bookmark
        {
            Title = "Docs",
            Url = "https://learn.microsoft.com",
            Description = "Microsoft Docs",
            Tags = "  Docs,DotNet,docs "
        });

        var all = await manager.ListAsync();

        Assert.Single(all);
        Assert.Equal("Docs", all[0].Title);
        Assert.Equal("docs,dotnet", all[0].Tags);
    }

    [Fact]
    public async Task GetByIdAsync_Throws_WhenMissing()
    {
        await using var dbContext = CreateDbContext();
        var manager = new BookmarkManager(dbContext);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => manager.GetByIdAsync(123));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesStoredValues()
    {
        await using var dbContext = CreateDbContext();
        var manager = new BookmarkManager(dbContext);

        var created = await manager.CreateAsync(new Bookmark
        {
            Title = "Old",
            Url = "https://example.com/old",
            Description = "Old description",
            Tags = "old"
        });

        created.Title = "New";
        created.Url = "https://example.com/new";
        created.Description = "New description";
        created.Tags = "new,tag";

        await manager.UpdateAsync(created);

        var fetched = await manager.GetByIdAsync(created.Id);
        Assert.Equal("New", fetched.Title);
        Assert.Equal("https://example.com/new", fetched.Url);
        Assert.Equal("new,tag", fetched.Tags);
    }

    [Fact]
    public async Task DeleteAsync_RemovesBookmark()
    {
        await using var dbContext = CreateDbContext();
        var manager = new BookmarkManager(dbContext);

        var created = await manager.CreateAsync(new Bookmark
        {
            Title = "Delete me",
            Url = "https://example.com/delete",
            Tags = "tmp"
        });

        await manager.DeleteAsync(created.Id);

        var all = await manager.ListAsync();
        Assert.Empty(all);
    }

    [Fact]
    public async Task CreateAsync_Throws_OnInvalidInput()
    {
        await using var dbContext = CreateDbContext();
        var manager = new BookmarkManager(dbContext);

        await Assert.ThrowsAsync<ArgumentException>(() => manager.CreateAsync(new Bookmark
        {
            Title = "",
            Url = "not-a-url"
        }));
    }

    [Fact]
    public async Task UpdateAsync_Throws_WhenBookmarkDoesNotExist()
    {
        await using var dbContext = CreateDbContext();
        var manager = new BookmarkManager(dbContext);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => manager.UpdateAsync(new Bookmark
        {
            Id = 999,
            Title = "Missing",
            Url = "https://example.com/missing"
        }));
    }

    private static BookmarkDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BookmarkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BookmarkDbContext(options);
    }
}

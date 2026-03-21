using AzenetOne.Bookmarks.Core.Entities;
using AzenetOne.Bookmarks.Core.Services;

namespace AzenetOne.Bookmarks.Core.Data;

public static class BookmarkSeeder
{
    public static async Task SeedDefaultsAsync(IBookmarkManager manager, CancellationToken cancellationToken = default)
    {
        var existing = await manager.ListAsync(cancellationToken);

        if (existing.Any(x => string.Equals(x.Url, "https://learn.microsoft.com/aspnet/core/blazor", StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        await manager.CreateAsync(new Bookmark
        {
            Title = "Blazor Docs",
            Url = "https://learn.microsoft.com/aspnet/core/blazor",
            Description = "Official Blazor documentation",
            Tags = "dotnet,blazor,docs"
        }, cancellationToken);

        await manager.CreateAsync(new Bookmark
        {
            Title = "EF Core Docs",
            Url = "https://learn.microsoft.com/ef/core",
            Description = "Entity Framework Core guides",
            Tags = "dotnet,efcore,docs"
        }, cancellationToken);
    }
}

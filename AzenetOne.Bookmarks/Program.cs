using AzenetOne.Bookmarks.Core.Data;
using AzenetOne.Bookmarks.Core.Entities;
using AzenetOne.Bookmarks.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("BookmarksDb")
	?? "Server=(localdb)\\MSSQLLocalDB;Database=AzenetOneBookmarks;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

builder.Services.AddDbContext<BookmarkDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IBookmarkManager, BookmarkManager>();

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<BookmarkDbContext>();
dbContext.Database.EnsureCreated();

var manager = scope.ServiceProvider.GetRequiredService<IBookmarkManager>();

await EnsureSampleAsync(manager);

var bookmarks = await manager.ListAsync();

Console.WriteLine("Bookmarks in database:");
foreach (var bookmark in bookmarks)
{
	Console.WriteLine($"- [{bookmark.Id}] {bookmark.Title} ({bookmark.Url}) | tags: {bookmark.Tags}");
}

static async Task EnsureSampleAsync(IBookmarkManager manager)
{
	var existing = await manager.ListAsync();
	if (existing.Any(x => string.Equals(x.Url, "https://github.com/dotnet/efcore", StringComparison.OrdinalIgnoreCase)))
	{
		return;
	}

	await manager.CreateAsync(new Bookmark
	{
		Title = "EF Core Source",
		Url = "https://github.com/dotnet/efcore",
		Description = "Entity Framework Core source code",
		Tags = "dotnet,efcore,github"
	});
}

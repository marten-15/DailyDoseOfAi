using AzenetOne.Bookmarks.Core.Data;
using AzenetOne.Bookmarks.Core.Services;
using AzenetOne.Bookmarks.Web.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("BookmarksDb")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=AzenetOneBookmarks;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

builder.Services.AddDbContext<BookmarkDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IBookmarkManager, BookmarkManager>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookmarkDbContext>();
    dbContext.Database.EnsureCreated();

    var bookmarkManager = scope.ServiceProvider.GetRequiredService<IBookmarkManager>();
    await BookmarkSeeder.SeedDefaultsAsync(bookmarkManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

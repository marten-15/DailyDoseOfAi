using AzenetOne.Bookmarks.Core.Entities;

namespace AzenetOne.Bookmarks.Core.Services;

public interface IBookmarkManager
{
    Task<Bookmark> CreateAsync(Bookmark bookmark, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Bookmark>> ListAsync(CancellationToken cancellationToken = default);
    Task<Bookmark> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Bookmark> UpdateAsync(Bookmark bookmark, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

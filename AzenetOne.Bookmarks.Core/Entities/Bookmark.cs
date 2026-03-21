using System.ComponentModel.DataAnnotations;

namespace AzenetOne.Bookmarks.Core.Entities;

public class Bookmark
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2048)]
    public string Url { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(1000)]
    public string? Tags { get; set; }

    public void Normalize()
    {
        Title = Title.Trim();
        Url = Url.Trim();
        Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();
        Tags = NormalizeTags(Tags);
    }

    public static string? NormalizeTags(string? rawTags)
    {
        if (string.IsNullOrWhiteSpace(rawTags))
        {
            return null;
        }

        var normalized = rawTags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.ToLowerInvariant())
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return normalized.Length == 0 ? null : string.Join(',', normalized);
    }
}

using System.ComponentModel.DataAnnotations;

namespace AzenetOne.Bookmarks.Core.Entities;

public class MTUserProfile
{
    [Required]
    [MaxLength(100)]
    public string Förnamn { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Efternamn { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Epost { get; set; } = string.Empty;

    [Required]
    public string[] Roller { get; set; } = [];

    [Required]
    [MaxLength(100)]
    public string Land { get; set; } = string.Empty;
}

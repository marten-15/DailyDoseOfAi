using System.ComponentModel.DataAnnotations;

namespace AzenetOne.Bookmarks.Core.Entities;

public class UserProfile
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string[] Roles { get; set; } = [];

    [MaxLength(100)]
    public string? Country { get; set; }
}

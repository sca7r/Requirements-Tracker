using System.ComponentModel.DataAnnotations;

namespace RequirementsTracker.Api.Models;

/// <summary>
/// A customer project for which a requirements specification is being maintained.
/// </summary>
public class Project
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<Requirement> Requirements { get; set; } = new();
}

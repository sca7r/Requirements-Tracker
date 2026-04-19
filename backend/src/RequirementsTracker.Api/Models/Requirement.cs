using System.ComponentModel.DataAnnotations;

namespace RequirementsTracker.Api.Models;

public enum RequirementType
{
    Functional,
    NonFunctional
}

/// <summary>
/// MoSCoW prioritisation — industry standard in requirements engineering.
/// </summary>
public enum Priority
{
    Must,
    Should,
    Could,
    Wont
}

public enum RequirementStatus
{
    Draft,
    Approved,
    InProgress,
    Done,
    Rejected
}

/// <summary>
/// A single requirement within a requirements specification.
/// </summary>
public class Requirement
{
    public int Id { get; set; }

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    [Required, MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    public RequirementType Type { get; set; } = RequirementType.Functional;
    public Priority Priority { get; set; } = Priority.Should;
    public RequirementStatus Status { get; set; } = RequirementStatus.Draft;

    /// <summary>Rough estimate in hours — supports analysis and effort estimation.</summary>
    public decimal? EstimatedHours { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

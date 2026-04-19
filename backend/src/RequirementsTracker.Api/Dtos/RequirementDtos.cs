using System.ComponentModel.DataAnnotations;
using RequirementsTracker.Api.Models;

namespace RequirementsTracker.Api.Dtos;

public record RequirementDto(
    int Id,
    int ProjectId,
    string Title,
    string? Description,
    RequirementType Type,
    Priority Priority,
    RequirementStatus Status,
    decimal? EstimatedHours,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateRequirementDto(
    [Required, MaxLength(300)] string Title,
    [MaxLength(4000)] string? Description,
    RequirementType Type,
    Priority Priority,
    decimal? EstimatedHours
);

public record UpdateRequirementDto(
    [Required, MaxLength(300)] string Title,
    [MaxLength(4000)] string? Description,
    RequirementType Type,
    Priority Priority,
    RequirementStatus Status,
    decimal? EstimatedHours
);

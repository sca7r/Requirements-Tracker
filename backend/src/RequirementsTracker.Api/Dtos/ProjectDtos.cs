using System.ComponentModel.DataAnnotations;

namespace RequirementsTracker.Api.Dtos;

public record ProjectDto(
    int Id,
    string Name,
    string CustomerName,
    string? Description,
    int RequirementCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateProjectDto(
    [Required, MaxLength(200)] string Name,
    [Required, MaxLength(200)] string CustomerName,
    [MaxLength(2000)] string? Description
);

public record UpdateProjectDto(
    [Required, MaxLength(200)] string Name,
    [Required, MaxLength(200)] string CustomerName,
    [MaxLength(2000)] string? Description
);

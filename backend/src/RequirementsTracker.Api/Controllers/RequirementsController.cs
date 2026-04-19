using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequirementsTracker.Api.Data;
using RequirementsTracker.Api.Dtos;
using RequirementsTracker.Api.Models;

namespace RequirementsTracker.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:int}/requirements")]
public class RequirementsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RequirementDto>>> GetForProject(int projectId)
    {
        var exists = await db.Projects.AnyAsync(p => p.Id == projectId);
        if (!exists) return NotFound();

        var reqs = await db.Requirements
            .Where(r => r.ProjectId == projectId)
            .OrderBy(r => r.Priority)
            .ThenBy(r => r.Id)
            .Select(r => new RequirementDto(
                r.Id, r.ProjectId, r.Title, r.Description,
                r.Type, r.Priority, r.Status, r.EstimatedHours,
                r.CreatedAt, r.UpdatedAt))
            .ToListAsync();

        return Ok(reqs);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RequirementDto>> Get(int projectId, int id)
    {
        var r = await db.Requirements.FirstOrDefaultAsync(x => x.Id == id && x.ProjectId == projectId);
        if (r is null) return NotFound();

        return Ok(new RequirementDto(
            r.Id, r.ProjectId, r.Title, r.Description,
            r.Type, r.Priority, r.Status, r.EstimatedHours,
            r.CreatedAt, r.UpdatedAt));
    }

    [HttpPost]
    public async Task<ActionResult<RequirementDto>> Create(int projectId, CreateRequirementDto dto)
    {
        var exists = await db.Projects.AnyAsync(p => p.Id == projectId);
        if (!exists) return NotFound();

        var r = new Requirement
        {
            ProjectId = projectId,
            Title = dto.Title,
            Description = dto.Description,
            Type = dto.Type,
            Priority = dto.Priority,
            EstimatedHours = dto.EstimatedHours,
            Status = RequirementStatus.Draft
        };

        db.Requirements.Add(r);
        await db.SaveChangesAsync();

        var result = new RequirementDto(
            r.Id, r.ProjectId, r.Title, r.Description,
            r.Type, r.Priority, r.Status, r.EstimatedHours,
            r.CreatedAt, r.UpdatedAt);

        return CreatedAtAction(nameof(Get), new { projectId = r.ProjectId, id = r.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RequirementDto>> Update(int projectId, int id, UpdateRequirementDto dto)
    {
        var r = await db.Requirements.FirstOrDefaultAsync(x => x.Id == id && x.ProjectId == projectId);
        if (r is null) return NotFound();

        r.Title = dto.Title;
        r.Description = dto.Description;
        r.Type = dto.Type;
        r.Priority = dto.Priority;
        r.Status = dto.Status;
        r.EstimatedHours = dto.EstimatedHours;
        r.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(new RequirementDto(
            r.Id, r.ProjectId, r.Title, r.Description,
            r.Type, r.Priority, r.Status, r.EstimatedHours,
            r.CreatedAt, r.UpdatedAt));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int projectId, int id)
    {
        var r = await db.Requirements.FirstOrDefaultAsync(x => x.Id == id && x.ProjectId == projectId);
        if (r is null) return NotFound();

        db.Requirements.Remove(r);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

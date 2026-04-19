using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RequirementsTracker.Api.Data;
using RequirementsTracker.Api.Dtos;
using RequirementsTracker.Api.Models;

namespace RequirementsTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
    {
        var projects = await db.Projects
            .Select(p => new ProjectDto(
                p.Id,
                p.Name,
                p.CustomerName,
                p.Description,
                p.Requirements.Count,
                p.CreatedAt,
                p.UpdatedAt))
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var p = await db.Projects
            .Include(x => x.Requirements)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (p is null) return NotFound();

        return Ok(new ProjectDto(
            p.Id, p.Name, p.CustomerName, p.Description,
            p.Requirements.Count, p.CreatedAt, p.UpdatedAt));
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto)
    {
        var p = new Project
        {
            Name = dto.Name,
            CustomerName = dto.CustomerName,
            Description = dto.Description
        };

        db.Projects.Add(p);
        await db.SaveChangesAsync();

        var result = new ProjectDto(p.Id, p.Name, p.CustomerName, p.Description, 0, p.CreatedAt, p.UpdatedAt);
        return CreatedAtAction(nameof(GetById), new { id = p.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, UpdateProjectDto dto)
    {
        var p = await db.Projects.FindAsync(id);
        if (p is null) return NotFound();

        p.Name = dto.Name;
        p.CustomerName = dto.CustomerName;
        p.Description = dto.Description;
        p.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        var count = await db.Requirements.CountAsync(r => r.ProjectId == id);
        return Ok(new ProjectDto(p.Id, p.Name, p.CustomerName, p.Description, count, p.CreatedAt, p.UpdatedAt));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await db.Projects.FindAsync(id);
        if (p is null) return NotFound();

        db.Projects.Remove(p);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

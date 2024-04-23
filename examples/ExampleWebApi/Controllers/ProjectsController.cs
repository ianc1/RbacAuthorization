namespace ExampleWebApi.Controllers;

using ExampleWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RbacAuthorization;
using RbacAuthorization.ExampleWebApi.Authorization;

[ApiController]
[Route("/organizations/{organizationId}/projects")]
public class ProjectsController : ControllerBase
{
    private readonly Repositories repositories;

    public ProjectsController(Repositories repositories)
    {
        this.repositories = repositories;
    }

    [HttpPost]
    [AuthorizePermission(Permissions.ProjectsCreate)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public ActionResult<ProjectDto> CreateTask(int organizationId, ProjectCreateDto dto)
    {
        if (!OrganizationExists(organizationId))
        {
            return NotFound();
        }

        var newProject = new ProjectDto(
            Id: repositories.Projects.Count + 1,
            OrganizationId: organizationId,
            Name: dto.Name);

        repositories.Projects.Add(newProject);

        return CreatedAtAction(nameof(GetProject), new{ organizationId, projectId = newProject.Id }, newProject);
    }

    [HttpGet]
    [AuthorizePermission(Permissions.ProjectsSearch)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public ActionResult<IEnumerable<ProjectDto>> SearchProjects(int organizationId)
    {
        if (!OrganizationExists(organizationId))
        {
            return NotFound();
        }

        var projects = repositories.Projects.Where(organization => organization.Id == organizationId);

        return Ok(projects);
    }

    [HttpGet("{projectId}")]
    [AuthorizePermission(Permissions.ProjectsRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public ActionResult<ProjectDto> GetProject(int organizationId, int projectId)
    {
        if (!OrganizationExists(organizationId))
        {
            return NotFound();
        }

        var project = repositories.Projects.FirstOrDefault(project =>
            project.Id == projectId
            && project.OrganizationId == organizationId);

        return project is null
            ? NotFound()
            : Ok(project);
    }

    [HttpDelete("{projectId}")]
    [AuthorizePermission(Permissions.ProjectsDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteProject(int organizationId, int projectId)
    {
        if (!OrganizationExists(organizationId))
        {
            return NotFound();
        }

        var project = repositories.Projects.RemoveAll(project =>
            project.Id == projectId
            && project.OrganizationId == organizationId);

        return NoContent();
    }

    private bool OrganizationExists(int organizationId) =>
        repositories.Organizations.Any(organization => organization.Id == organizationId);
}

namespace ExampleWebApi.Controllers;

using ExampleWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RbacAuthorization;
using RbacAuthorization.ExampleWebApi.Authorization;

[ApiController]
[Route("/organizations/{organizationId}/projects/{projectId}/tasks")]
public class TasksController : ControllerBase
{
    private readonly Repositories repositories;

    public TasksController(Repositories repositories)
    {
        this.repositories = repositories;
    }

    [HttpPost]
    [AuthorizePermission(Permissions.TasksCreate)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public ActionResult<TaskDto> CreateTask(int organizationId, int projectId, TaskCreateDto dto)
    {
        if (!ProjectExists(projectId, organizationId))
        {
            return NotFound();
        }

        var newTask = new TaskDto(
            Id: repositories.Tasks.Count + 1,
            OrganizationId: organizationId,
            ProjectId: projectId,
            Title: dto.Title);

        repositories.Tasks.Add(newTask);

        return CreatedAtAction(nameof(GetTask), new { organizationId, projectId, taskId = newTask.Id }, newTask);
    }

    [HttpGet]
    [AuthorizePermission(Permissions.TasksSearch)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public ActionResult<IEnumerable<TaskDto>> SearchTasks(int organizationId, int projectId)
    {
        if (!ProjectExists(projectId, organizationId))
        {
            return NotFound();
        }

        var projectTasks = repositories.Tasks.Where(task =>
            task.OrganizationId == organizationId
            && task.ProjectId == projectId);

        return Ok(projectTasks);
    }

    [HttpGet("{taskId}")]
    [AuthorizePermission(Permissions.TasksRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public ActionResult<TaskDto> GetTask(int organizationId, int projectId, int taskId)
    {
        if (!ProjectExists(projectId, organizationId))
        {
            return NotFound();
        }

        var projectTasks = repositories.Tasks.Where(task =>
            task.OrganizationId == organizationId
            && task.ProjectId == projectId
            && task.Id == taskId);

        return Ok(projectTasks);
    }

    [HttpDelete("{taskId}")]
    [AuthorizePermission(Permissions.TasksDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public IActionResult DeleteTask(int organizationId, int projectId, int taskId)
    {
        if (!ProjectExists(projectId, organizationId))
        {
            return NotFound();
        }

        repositories.Tasks.RemoveAll(task =>
            task.OrganizationId == organizationId
            && task.ProjectId == projectId
            && task.Id == taskId);

        return NoContent();
    }

    private bool ProjectExists(int projectId, int organizationId) =>
        repositories.Projects.Any(project => project.Id == projectId && project.OrganizationId == organizationId);
}

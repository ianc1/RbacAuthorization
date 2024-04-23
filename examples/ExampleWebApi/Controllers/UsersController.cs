namespace ExampleWebApi.Controllers;

using ExampleWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RbacAuthorization;
using RbacAuthorization.ExampleWebApi.Authorization;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly Repositories repositories;

    public UsersController(Repositories repositories)
    {
        this.repositories = repositories;
    }

    [HttpGet("me")]
    [AuthorizePermission(Permissions.UsersRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetUserMe()
    {
        var me = repositories.Users[0];

        return Ok(me);
    }

    [HttpGet]
    [AuthorizePermission(Permissions.UsersSearch)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<UserDto>> SearchUsers()
    {
        return Ok(repositories.Users);
    }

    [HttpGet("{userId}")]
    [AuthorizePermission(Permissions.UsersRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public ActionResult<UserDto> GetUser(int userId)
    {
        var user = repositories.Users.FirstOrDefault(user => user.Id == userId);

        return user is null
            ? NotFound()
            : Ok(user);
    }
}

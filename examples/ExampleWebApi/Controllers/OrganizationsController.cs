namespace ExampleWebApi.Controllers;

using ExampleWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RbacAuthorization;
using RbacAuthorization.ExampleWebApi.Authorization;

[ApiController]
[Route("/organizations")]
public class OrganizationsController : ControllerBase
{
    private readonly Repositories repositories;

    public OrganizationsController(Repositories repositories)
    {
        this.repositories = repositories;
    }

    [HttpGet]
    [AuthorizePermission(Permissions.OrganizationsSearch)]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public ActionResult<IEnumerable<OrganizationDto>> SearchOrganizations()
    {
        return Ok(repositories.Organizations);
    }

    [HttpGet("{organizationId}")]
    [AuthorizePermission(Permissions.OrganizationsRead)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public ActionResult<OrganizationDto> GetOrganizations(int organizationId)
    {
        var organization = repositories.Organizations.FirstOrDefault(organization => organization.Id == organizationId);

        return organization is null
            ? NotFound()
            : Ok(organization);
    }
}

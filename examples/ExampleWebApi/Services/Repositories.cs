namespace ExampleWebApi.Services;

public class Repositories
{
    public List<UserDto> Users { get; } = new List<UserDto>
    {
        new UserDto(Id: 1, Name: "JDoe"),
        new UserDto(Id: 2, Name: "JSmith"),
    };

    public List<OrganizationDto> Organizations { get; } = new List<OrganizationDto>
    {
        new OrganizationDto(Id: 1, "Acme"),
        new OrganizationDto(Id: 2, " Contoso")
    };

    public List<ProjectDto> Projects { get; } = new List<ProjectDto>
    {
        new ProjectDto(Id: 1, OrganizationId: 1, "Sales"),
        new ProjectDto(Id: 2, OrganizationId: 1, "Engineering"),
        new ProjectDto(Id: 3, OrganizationId: 2, "Marketing"),
    };

    public List<TaskDto> Tasks { get; } = new List<TaskDto>
    {
        new TaskDto(Id: 1, OrganizationId: 1, ProjectId: 1, Title: "Close deal x"),
        new TaskDto(Id: 2, OrganizationId: 1, ProjectId: 1, Title: "Close deal y"),
        new TaskDto(Id: 3, OrganizationId: 1, ProjectId: 2, Title: "Create feature x"),
        new TaskDto(Id: 4, OrganizationId: 2, ProjectId: 3, Title: "Create advert on x"),
    };
}

public record OrganizationDto(int Id, string Name);

public record ProjectCreateDto(string Name);

public record ProjectDto(int Id, int OrganizationId, string Name);

public record TaskCreateDto(string Title);

public record TaskDto(int Id, int OrganizationId, int ProjectId, string Title);

public record UserDto(int Id, string Name);

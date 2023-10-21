namespace RbacAuthorization.ExampleWebApi;

public static class Roles
{
    public const string TenantSupervisor = "MyApp.$TenantId.Supervisor";

    public const string TenantAssistant = "MyApp.$TenantId.Assistant";

    public const string HelpDesk = "MyApp.HelpDesk";
}

namespace RbacAuthorization.Tests.TestHarness
{
    public class TestValues
    {
        public const string TestUserId = "5c9b2677-a4f9-4e23-a4d6-29ffb661fbcd";

        public const string WritePermission = "Tasks.Write";

        public const string ReadPermission = "Tasks.Read";

        public const string PermissionPolicyNamePrefix = "PermissionPolicy:";

        public const string WritePermissionPolicy = $"{PermissionPolicyNamePrefix}{WritePermission}";

        public const string ReadPermissionPolicy = $"{PermissionPolicyNamePrefix}{ReadPermission}";

        public const string AdminRoleName = "Admin";

        public const string UserRoleName = "User";

        public const string UserScope = "/users/me";

        public const string ProjectAdminRoleName = "ProjectAdmin";

        public const string ProjectAdminScopeDefinition = "/organizations/{OrganizationId}/projects/{ProjectId}";

        public const string ProjectAdminScope = "/organizations/123/projects/456";

        public const string ProjectAdminRole = $"{ProjectAdminRoleName}:{ProjectAdminScope}";
    }
}

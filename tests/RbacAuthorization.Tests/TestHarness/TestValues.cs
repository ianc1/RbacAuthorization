namespace RbacAuthorization.Tests.TestHarness
{
    public class TestValues
    {
        public const string TestUserId = "5c9b2677-a4f9-4e23-a4d6-29ffb661fbcd";

        public const string TestTenantId = "cfbe267c-90fd-4ffd-88c6-97f824a0caf5";

        public const string TestTenantSupervisorRole = $"TestApp.{TestTenantId}.Supervisor";

        public const string TestTenantManagerRole = $"TestApp.{TestTenantId}.Manager";

        public const string WritePermission = "Tasks.Write";

        public const string ReadPermission = "Tasks.Read";

        public const string SupervisorRole = "TestApp.Supervisor";

        public const string ManagerRole = "TestApp.Manager";

        public const string TenantSupervisorRole = "TestApp.$TenantId.Supervisor";

        public const string TenantManagerRole = "TestApp.$TenantId.Manager";
    }
}

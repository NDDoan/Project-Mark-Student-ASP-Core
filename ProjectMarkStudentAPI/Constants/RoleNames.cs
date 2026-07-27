namespace ProjectMarkStudentAPI.Constants
{
    /// <summary>
    /// Centralized role name constants — avoid magic strings spread across controllers.
    /// Values must match exactly the RoleName column in the Roles table.
    /// </summary>
    public static class RoleNames
    {
        public const string Admin   = "Admin";
        public const string Manager = "Quản Lý";
        public const string Teacher = "Teacher";
    }
}

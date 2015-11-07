namespace SoniReports.DataAccessLayer
{    
    public static class SoniRoles
    {
        public const string Admin = "Admin";
        public const string ReportViewer = "ReportViewer";
        public const string SuperUser = "SuperUser";
        public const string AllCommaSeparated = "SuperUser, Admin, ReportViewer";
        public const string ManagersCommaSeparated = "SuperUser, Admin";

        public static string[] All = { SuperUser, Admin, ReportViewer };
        public static string[] AllExceptManagers = { ReportViewer };
        public static string[] AllExceptSuperUser = { Admin, ReportViewer };
        public static string[] Managers = { SuperUser, Admin };
    }

}

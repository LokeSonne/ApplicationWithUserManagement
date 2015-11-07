using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoniReports.DomainModel
{
    public enum ActivityKind { View = 1 }
    public enum ObjectKind { Report = 1 }

    public static class ActivityKindExt {
        public static string Caption(this ActivityKind kind) { return "Vis"; }    
    }

    public static class ObjectKindExt {
        public static string Caption(this ObjectKind kind) { return "Rapport"; }
    }

    public class UserActivityLogEntry
    {
        public DateTime TimeStamp { get; set; }
        public string Id { get; set; }
        public string UserId { get; set; }

        public ActivityKind ActivityKind { get; set; }
        public ObjectKind ObjectKind { get; set; }
    }
}
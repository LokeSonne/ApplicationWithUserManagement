using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;

namespace SoniReports.Tools
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum e)
        {
            var rm = new ResourceManager(typeof(ResourceDK));
            var resourceDisplayName = rm.GetString(/*e.GetType().Name + "_" +*/ e.ToString());

            return string.IsNullOrWhiteSpace(resourceDisplayName) ? string.Format("[[{0}]]", e) : resourceDisplayName;
        }
    }
}

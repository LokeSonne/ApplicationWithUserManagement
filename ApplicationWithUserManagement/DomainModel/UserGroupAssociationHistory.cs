using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SoniReports.Controllers;
using System.Collections.Generic;
using System;

namespace SoniReports.DomainModel
{
    public class UserGroupAssociationHistory
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserGroupId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}

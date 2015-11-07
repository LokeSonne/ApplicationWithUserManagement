using Microsoft.AspNet.Identity;
using SoniReports.DataAccessLayer;
using SoniReports.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoniReports.ViewModels
{
    public class GroupViewModel
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public bool Assigned { get; set; }
    
        internal static GroupViewModel From(UserGroup group, bool assigned)
        {
            return new GroupViewModel
            {
                GroupId = group.Id,
                GroupName = group.Name, 
                Assigned = assigned
            };
        }
    }
}

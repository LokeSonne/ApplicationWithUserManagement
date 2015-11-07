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
    public class RoleViewModel
    {
        public string RoleName { get; set; }
        public bool Assigned { get; set; }

        internal static RoleViewModel From(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role, bool assigned)
        {
            return From(role.Name, assigned);
        }

        internal static RoleViewModel From(string roleName, bool assigned)
        {
            return new RoleViewModel
            {
                RoleName = roleName, 
                Assigned = assigned
            };
        }
    }

    public class AssignRolesViewModel
    {
        public string UserId { get; set; }
        
        [Display(Name="Brugernavn")]
        public string UserName { get; set; }
        public List<RoleViewModel> Roles { get; private set; }
        
        public IdentityResult Result;

        public AssignRolesViewModel()
        {
            Roles = new List<RoleViewModel>();
        }

        internal static AssignRolesViewModel From(ApplicationUser user, List<RoleViewModel> roleList)
        {
            return new AssignRolesViewModel { UserId = user.Id, UserName = user.UserName, Roles = roleList };
        }
    }

}
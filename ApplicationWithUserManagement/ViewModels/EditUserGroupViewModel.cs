using Microsoft.AspNet.Identity;
using SoniReports.DataAccessLayer;
using SoniReports.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoniReports.ViewModels
{
    public class EditUserGroupViewModel
    {
        public string Id { get; set; }
        
        [Required]
        [Display(Name ="Navn")]
        public string Name { get; set; }

        [Display(Name ="Aktiv")]
        public bool Enabled { get; set; }

        public bool IsNewUserGroup;

        public IdentityResult Result;

        internal static EditUserGroupViewModel From(UserGroup userGroup)
        {
            return new EditUserGroupViewModel { Id = userGroup.Id, Name = userGroup.Name, Enabled = userGroup.Enabled };
        }

        internal static EditUserGroupViewModel FromParams(string id, bool isNewUserGroup, string name, bool enabled)
        {
            return new EditUserGroupViewModel { Id = id, IsNewUserGroup = isNewUserGroup, Name = name, Enabled = enabled };
        }


        internal void WriteUpdatesTo(UserGroup group)
        {
            group.Name = Name;
            group.Enabled = Enabled;
        }

        internal UserGroup To()
        {
            var result = new UserGroup { Id = Id, Name = Name, Enabled= Enabled };
            return result;
        }
    }

}
using Microsoft.AspNet.Identity;
using SoniReports.DomainModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SoniReports.ViewModels
{
    public class AssignGroupsViewModel
    {
        public string UserId { get; set; }

        [Display(Name = "Brugernavn")]
        public string UserName { get; set; }
        public List<GroupViewModel> UserGroups { get; private set; }
        
        public IdentityResult Result;


        public AssignGroupsViewModel()
        {
            UserGroups = new List<GroupViewModel>();
        }

        internal static AssignGroupsViewModel From(ApplicationUser user, List<GroupViewModel> groupList)
        {
            return new AssignGroupsViewModel { UserId = user.Id, UserName = user.UserName, UserGroups = groupList };
        }

        public string SingleSelectedGroup
        {
            set
            {
                UserGroups.ForEach(g => { g.Assigned = g.GroupId == value; });
            }
            get
            {
                var selected =  UserGroups.SingleOrDefault(g => g.Assigned);
                return selected == null ? null : selected.GroupId;
            }
        }
    }

}
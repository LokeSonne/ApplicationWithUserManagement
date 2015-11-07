using System.Collections.Generic;

namespace SoniReports.DomainModel
{
    public class UserGroupAssociation
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string UserGroupId { get; set; }
        public virtual UserGroup UserGroup { get; set; }
    
        //public virtual ICollection<UserGroupAssociation> UserGroupAssociations { get; set; }
    }
}
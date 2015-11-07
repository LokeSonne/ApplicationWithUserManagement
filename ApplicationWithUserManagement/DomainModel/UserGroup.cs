using System.Collections.Generic;

namespace SoniReports.DomainModel
{
    public class UserGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }

        public virtual ICollection<UserGroupAssociation> UserGroupAssociations { get; set; }
    }
}

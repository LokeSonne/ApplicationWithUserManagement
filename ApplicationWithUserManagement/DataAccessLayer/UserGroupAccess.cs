using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using SoniReports.DomainModel;
using System.Data.Entity;


namespace SoniReports.DataAccessLayer
{
    public class UserGroupAccess
    {
        public IEnumerable<UserGroup> GetUserGroups()
        {
            var result = ApplicationDbContext.DbAction(db => db.UserGroups
                .OrderBy(g => g.Enabled)
                .ThenBy(g => g.Name)
                .ToList());
            return result;
        }

        public IEnumerable<ApplicationUser> GetUsersInGroup(string userGroupId)
        {           
            var result = ApplicationDbContext.DbAction(db => db.UserGroupAssociations
                    .Where(a => a.UserGroupId == userGroupId)
                    .Join(db.Users, a => a.UserId, i => i.Id, (a, i) => i)
                    .OrderBy(u => u.UserName)
                    .ToList());
            return result;
        }

        public IEnumerable<UserGroup> GetGroupsForUser(string userId)
        {
            var result = ApplicationDbContext.DbAction(db => db.UserGroupAssociations
                    .Where(a => a.UserId == userId)
                    .Join(db.UserGroups, a => a.UserGroupId, i => i.Id, (a, i) => i)
                    .OrderBy(u => u.Enabled)
                    .ThenBy(u => u.Name)
                    .ToList());
            return result;
        }

        internal UserGroup FindById(string id)
        {
            var result = ApplicationDbContext.DbAction(db => db.UserGroups.SingleOrDefault(g => g.Id == id));
            return result;
        }

        //-------------------------------------------------------------------------------------------------------

        internal int Create(UserGroup userGroup)
        {
            int result = ApplicationDbContext.DbAction(db =>
                {
                    db.UserGroups.Add(userGroup);
                    return db.SaveChanges();
                });
            return result;
        }

        internal int Update(string userGroupId, Action<UserGroup> setter)
        {
            var result = ApplicationDbContext.DbAction(db =>
            {
                var userGroup = db.UserGroups.SingleOrDefault(g => g.Id == userGroupId);
                setter(userGroup);
                return db.SaveChanges();
            });
            return result;
        }

        internal int Delete(string id)
        {
            int result = ApplicationDbContext.DbAction(db =>
            {
                var userGroup = db.UserGroups.SingleOrDefault(g => g.Id == id);
                if (userGroup == null) return 0;
                db.UserGroups.Remove(userGroup);
                return db.SaveChanges();
            });
            return result;
        }

        //-------------------------------------------------------------------------------------------------------

        internal int AddUserToGroup(string userId, string userGroupId)
        {
            var result = ApplicationDbContext.DbAction(db =>
              {
                  var association = new UserGroupAssociation { Id = Guid.NewGuid().ToString(), UserId = userId, UserGroupId = userGroupId };
                  db.UserGroupAssociations.Add(association);
                  return db.SaveChanges();
              });

            RegisterAssociationStart(userId, userGroupId, DateTime.Now);

            return result;
        }

        internal int RemoveUserFromGroup(string userId, string userGroupId)
        {
              var result = ApplicationDbContext.DbAction(db =>
                {
                    var associations = db.UserGroupAssociations.Where(a => a.UserId == userId && a.UserGroupId == userGroupId);
                    foreach (var association in associations)
                    {
                        db.UserGroupAssociations.Remove(association);
                    }
                    return db.SaveChanges();
              });

              RegisterAssociationEnd(userId, userGroupId, DateTime.Now);

            return result;
        }

        //-------------------------------------------------------------------------------------------------------
        internal int RegisterAssociationStart(string userId, string userGroupId, DateTime from)
        {
            var result = ApplicationDbContext.DbAction(db =>
            {
                var entry = new UserGroupAssociationHistory { Id = Guid.NewGuid().ToString(), 
                    UserId = userId, UserGroupId = userGroupId, From = from, To = (DateTime) SqlDateTime.MaxValue};
                db.UserGroupAssociationHistory.Add(entry);
                return db.SaveChanges();
            });
            return result;
        }

        internal int RegisterAssociationEnd(string userId, string userGroupId, DateTime to)
        {
            var result = ApplicationDbContext.DbAction(db =>
            {
                var entry = db.UserGroupAssociationHistory.Single(e => e.UserId == userId && e.UserGroupId == userGroupId &&
                    e.From <= to && e.To >= to);
                entry.To = to;
                return db.SaveChanges();
            });
            return result;
        }

        internal List<UserGroupAssociationHistory> GetUserGroupAssociationHistory()
        {
            var result = ApplicationDbContext.DbAction(db =>
                {
                    var history = db.UserGroupAssociationHistory.ToList();
                    return history;
                });
            return result;
        }
    }
}
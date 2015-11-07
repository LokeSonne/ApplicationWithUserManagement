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
    public class EditUserViewModel
    {
        public string Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name ="Email bekræftet")]
        public bool EmailConfirmed { get; set; }

        [Display(Name ="Aktiveret")]
        public bool Activated { get; set; }

        public bool IsNewUser { get; set; }

        public IdentityResult Result;

        public ApplicationUser To()
        {
            return new ApplicationUser { 
                Id = Id,
                Email = Email, 
                EmailConfirmed = EmailConfirmed, 
                Activated = Activated ? ActivationStatus.Activated : ActivationStatus.Rejected,
                UserName = Email
            };
        }

        public void WriteUpdatesTo(ApplicationUser user)
        {
            user.Email = Email;
            user.EmailConfirmed = EmailConfirmed;
            user.Activated = Activated ? ActivationStatus.Activated : ActivationStatus.Rejected;
            user.UserName = Email;
        }

        public static EditUserViewModel From(ApplicationUser user)
        {
            return new EditUserViewModel { 
                Id = user.Id,
                Email= user.Email, 
                EmailConfirmed = user.EmailConfirmed, 
                Activated = user.Activated == ActivationStatus.Activated
            };
        }

        internal static EditUserViewModel FromParams(string id, bool isNewUser, string email, bool emailConfirmed, bool activated)
        {
            return new EditUserViewModel
            {
                Id = id,
                IsNewUser = isNewUser,
                Email = email,
                EmailConfirmed = emailConfirmed,
                Activated = activated
            };
        }
    }

}
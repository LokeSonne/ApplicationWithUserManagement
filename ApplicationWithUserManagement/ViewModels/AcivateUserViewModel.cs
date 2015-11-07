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
    public class ActivateUserViewModel
    {
        public string Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name ="Email bekræftet")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Roller")]
        public List<string> Roles { get; set; }

        public IdentityResult Result;
       
        public static ActivateUserViewModel From(ApplicationUser user, List<string> roles)
        {
            return new ActivateUserViewModel
            { 
                Id = user.Id,
                Email= user.Email, 
                EmailConfirmed = user.EmailConfirmed, 
               Roles = roles
            };
        }

    }

}
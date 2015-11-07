using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoniReports.ViewModels
{
    public class AdminSetPasswordViewModel
    {
        public string Id { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} skal være mindst {2} karakterer langt", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name ="Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name ="Bekræft password")]
        [Compare("Password", ErrorMessage = "Password og bekræftelsespassword er ikke ens.")]
        public string ConfirmPassword { get; set; }

        internal static AdminSetPasswordViewModel From(DomainModel.ApplicationUser targetUser)
        {
            return new AdminSetPasswordViewModel { Id = targetUser.Id, Email = targetUser.Email };
        }
    }
}
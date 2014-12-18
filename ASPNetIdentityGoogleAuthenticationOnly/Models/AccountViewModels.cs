using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASPNetIdentityGoogleAuthenticationOnly.Models
{
    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}
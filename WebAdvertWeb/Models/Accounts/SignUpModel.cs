using System.ComponentModel.DataAnnotations;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace WebAdvertWeb.Models.Accounts
{
    public class SignUpModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be at least 6 and at most 30 characters")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and its confirmation do not match")]
        public string ConfirmPassword { get; set; }
    }
}
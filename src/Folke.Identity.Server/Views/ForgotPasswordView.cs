using System.ComponentModel.DataAnnotations;

namespace Folke.Identity.Server.Views
{
    public class ForgotPasswordView
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

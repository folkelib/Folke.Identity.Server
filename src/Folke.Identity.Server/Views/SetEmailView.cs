using System.ComponentModel.DataAnnotations;

namespace Folke.Identity.Server.Views
{
    public class SetEmailView
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

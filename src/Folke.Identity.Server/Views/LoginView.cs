using System.ComponentModel.DataAnnotations;

namespace Folke.Identity.Server.Views
{
    public class LoginView
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}

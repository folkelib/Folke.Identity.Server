using System.ComponentModel.DataAnnotations;

namespace Folke.Identity.Server.Views
{
    public class AuthenticationSchemeView
    { 
        /// <summary>The name of the authentication scheme.</summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The display name for the scheme. Null is valid and used for non user facing schemes.
        /// </summary>
        public string DisplayName { get; set; }
    }
}

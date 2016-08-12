using System.ComponentModel.DataAnnotations;

namespace Folke.Identity.Server.Views
{
    public class BaseUserView<TKey>
    {
        [Required]
        public string UserName { get; set; }
        public bool Logged { get; set; }
        public bool EmailConfirmed { get; set; }
        [Required]
        public string Email { get; set; }
        public TKey Id { get; set; }
        public bool HasPassword { get; set; }
    }
}

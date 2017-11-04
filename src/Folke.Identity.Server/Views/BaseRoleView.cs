using System.ComponentModel.DataAnnotations;

namespace Folke.Identity.Server.Views
{
    public class BaseRoleView<TKey>
    {
        public TKey Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}

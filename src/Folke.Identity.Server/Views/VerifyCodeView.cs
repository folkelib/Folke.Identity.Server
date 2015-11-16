using System.ComponentModel.DataAnnotations;

namespace Folke.Identity.Server.Views
{
    public class VerifyCodeView
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        public string Code { get; set; }

        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }
}

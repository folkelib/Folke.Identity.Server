using System.Collections.Generic;

namespace Folke.Identity.Server.Views
{
    public class SendCodeView
    {
        public string SelectedProvider { get; set; }
        public ICollection<string> Providers { get; set; }
        public bool RememberMe { get; set; }
    }
}

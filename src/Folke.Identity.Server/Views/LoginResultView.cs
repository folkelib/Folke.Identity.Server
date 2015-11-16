using Folke.Identity.Server.Enumeration;

namespace Folke.Identity.Server.Views
{
    public class LoginResultView
    {
        public LoginStatusEnum Status { get; set; }
        public bool RememberMe { get; set; }
    }
}

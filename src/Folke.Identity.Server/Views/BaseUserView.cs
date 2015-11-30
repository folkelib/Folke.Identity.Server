namespace Folke.Identity.Server.Views
{
    public class BaseUserView<TKey>
    {
        public string UserName { get; set; }
        public bool Logged { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Email { get; set; }
        public TKey Id { get; set; }
        public bool HasPassword { get; set; }
    }
}

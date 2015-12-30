namespace Folke.Identity.Server.Views
{
    public class BaseRoleView<TKey>
    {
        public TKey Id { get; set; }
        public string Name { get; set; }
    }
}

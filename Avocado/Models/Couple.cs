
namespace Avocado.Models
{
    public class Couple
    {
        public string Id { get; set; }
        public User CurrentUser { get; set; }
        public User OtherUser { get; set; }
    }
}

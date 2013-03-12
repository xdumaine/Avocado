
namespace Avocado.Models
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public long Birthday { get; set; }
        public string email { get; set; }
        public string CurrentCoupleId { get; set; }
        public string AvatarUrl { get; set; }
        public string AvatarUrlSmall { get; set; }
        public string AvatarUrlMedium { get; set; }
        public bool Verified { get; set; }
    }
}

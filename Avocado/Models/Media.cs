
namespace Avocado.Models
{
    public class Media
    {
        public string Id { get; set; }
        public string Caption { get; set; }
        public string Url { get; set; }
        public long TimeCreated { get; set; }
        public long TimeUpdated { get; set; }
        public string FileName { get; set; }
        public ImageUrlCollection ImageUrls { get; set; }
        public PhotoInfo Info { get; set; }
    }
}

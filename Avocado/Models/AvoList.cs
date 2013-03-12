using System.Collections.ObjectModel;
using Avocado.ViewModels;

namespace Avocado.Models
{
    public class AvoList
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long TimeCreated { get; set; }
        public long TimeUpdated { get; set; }
        public ObservableCollection<AvoListItem> Items { get; set; }
    }
}

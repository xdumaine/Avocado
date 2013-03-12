using Avocado.Models;
using MetroMVVM;

namespace Avocado.ViewModels
{
    public class AvoListItem : ObservableObject
    {
        #region Observables

        private bool complete;
        public bool Complete
        {
            get
            {
                return complete;
            }
            set
            {
                complete = value;
                RaisePropertyChanged("Complete");
            }
        }

        private string text;
        public string Text { get { return text; } set { text = value; RaisePropertyChanged("Text"); } }

        private bool important;
        public bool Important { get { return important; } set { important = value; RaisePropertyChanged("Important"); } }

        #endregion

        public string Id { get; set; }
        public string ListId { get; set; }
        public string UserId { get; set; }
        public ImageUrlCollection ImageUrls { get; set; }
        public PhotoInfo ImageInfo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avocado.DataModel;
using MetroMVVM;
using MetroMVVM.Commands;
using Windows.System;
using Windows.UI.Xaml.Input;

namespace Avocado.ViewModel
{
    public enum TabType
    {
        Activities, 
        Lists, 
        Photos, 
        Preferences,
        Calendar
    }

    class Activities : ObservableObject
    {
        public AuthClient AuthClient { get; set; }

        private CoupleModel couple;
        public CoupleModel Couple { get { return couple; } set { couple = value; RaisePropertyChanged("Couple"); RaisePropertyChanged("NewMessagePrompt"); } }

        private List<Activity> activityList;
        public List<Activity> ActivityList { get { return activityList; } set { activityList = value; RaisePropertyChanged("ActivityList"); } }

        private List<ListModel> listModelList;
        public List<ListModel> ListModelList { get { return listModelList; } set { listModelList = value; RaisePropertyChanged("ListModelList"); } }
        
        private ListModel selectedListModel;
        public ListModel SelectedListModel { 
            get 
            { 
                return selectedListModel; 
            } 
            set 
            {
                if (value == null)
                {
                    return;
                }
                // The value passed will not equal an actual list object in the list model list, so find
                // the real one with linq by the Id, and use that one.
                selectedListModel = (from list in ListModelList
                                     where list.Id == value.Id
                                     select list).First();
                SelectedTab = TabType.Lists;
                RaisePropertyChanged("SelectedListModel"); 
            } 
        }

        private string newMessage;
        public string NewMessage { get { return newMessage; } set { newMessage = value; RaisePropertyChanged("NewMessage"); } }

        public string NewMessagePrompt
        {
            get
            {
                return string.Format("Send {0} a message...", Couple.OtherUser.FirstName);
            }
        }
        public void NewMessageKeyDown(KeyRoutedEventArgs args)
        {
            if (args.Key == VirtualKey.Enter)
            {
                if (!string.IsNullOrEmpty(NewMessage))
                {
                    IsLoading = true;
                    AuthClient.SendMessage(NewMessage);
                    IsLoading = false;
                }
                NewMessage = "";
                Update();
            }
        }
        public ICommand NewMessageKeyDownCommand
        {
            get
            {
                return new RelayCommand<KeyRoutedEventArgs>(args => { NewMessageKeyDown(args); });
            }
        }

        private Activity selectedActivity;
        public Activity SelectedActivity 
        { 
            get 
            { 
                return selectedActivity; 
            } 
            set 
            { 
                selectedActivity = value;
                if (value == null)
                {
                    return;
                }
                if (selectedActivity.Type == "list")
                {
                    SelectedListModel = AuthClient.GetList(selectedActivity.Data.Id);
                }
                RaisePropertyChanged("SelectedActivity");
            } 
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(() => Update() );
            }
        }

        private bool isLoading;
        public bool IsLoading { get { return isLoading; } set { isLoading = value; RaisePropertyChanged("IsLoading"); } }

        #region ListItemActions

        public void EditListItem(ListItemModel listItem)
        {
            IsLoading = true;
            AuthClient.EditListItem(listItem);
            IsLoading = false;
        }

        public ICommand EditListItemCommand
        {
            get
            {
                return new RelayCommand<ListItemModel>(param => EditListItem(param));
            }
        }

        public void ToggleListItemImportant(ListItemModel listItem)
        {
            listItem.Important = !listItem.Important;
            EditListItem(listItem);
        }

        public ICommand ToggleListItemImportantCommand
        {
            get
            {
                return new RelayCommand<ListItemModel>(param => { ToggleListItemImportant(param); });
            }
        }

        #endregion

        #region SelectedTab
        private TabType selectedTab;
        public TabType SelectedTab
        {
            get
            {
                return selectedTab;
            }
            set
            {
                selectedTab = value;
                RaisePropertyChanged("SelectedTab");
                RaisePropertyChanged("IsActivityTabActive");
                RaisePropertyChanged("IsPhotoTabActive");
                RaisePropertyChanged("IsListTabActive");
                RaisePropertyChanged("IsCalendarTabActive");
                RaisePropertyChanged("IsPreferenceTabActive");
            }
        }
        
        public bool IsListTabActive
        {
            get
            {
                return SelectedTab == TabType.Lists;
            }
        }
        
        public bool IsPhotoTabActive
        {
            get
            {
                return SelectedTab == TabType.Photos;
            }
        }
        
        public bool IsActivityTabActive
        {
            get
            {
                return SelectedTab == TabType.Activities;
            }
        }
        
        public bool IsPreferenceTabActive
        {
            get
            {
                return SelectedTab == TabType.Preferences;
            }
        }

        public bool IsCalendarTabActive
        {
            get
            {
                return SelectedTab == TabType.Calendar;
            }
        }

        public void SetTabActive(TabType tab)
        {
            SelectedTab = tab;
        }

        public ICommand SetActivityTabActiveCommand
        {
            get
            {
                return new RelayCommand(() => { SetTabActive(TabType.Activities); });
            }
        }

        public ICommand SetPhotoTabActiveCommand
        {
            get
            {
                return new RelayCommand(() => { SetTabActive(TabType.Photos); });
            }
        }

        public ICommand SetListTabActiveCommand
        {
            get
            {
                return new RelayCommand(() => { SetTabActive(TabType.Lists); });
            }
        }

        public ICommand SetPreferenceTabActiveCommand
        {
            get
            {
                return new RelayCommand(() => { SetTabActive(TabType.Preferences); });
            }
        }

        public ICommand SetCalendarTabActiveCommand
        {
            get
            {
                return new RelayCommand(() => { SetTabActive(TabType.Calendar); }); 
            }
        }
        #endregion

        public Activities()
        {
           
        }

        public void Update()
        {
            IsLoading = true;
            Couple = AuthClient.GetUsers();
            ActivityList = AuthClient.GetActivities();
            ListModelList = AuthClient.GetListModelList();

            foreach (var activity in ActivityList)
            {
                activity.User = activity.UserId == Couple.CurrentUser.Id ? Couple.CurrentUser : Couple.OtherUser;
            }
            ActivityList.Reverse();
            SelectedActivity = ActivityList.First();
            IsLoading = false;
        }
    }
}

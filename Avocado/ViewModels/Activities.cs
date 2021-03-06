﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avocado.Models;
using Avocado.ViewModels;
using MetroMVVM;
using MetroMVVM.Commands;
using MetroMVVM.Threading;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using WinRTXamlToolkit.Controls;

namespace Avocado.ViewModel
{
    public enum TabType
    {
        Activities, 
        Lists, 
        Media, 
        Preferences,
        Calendar
    }

    class Activities : ObservableObject
    {
        #region Constants
        
        public const string PostButtonText = "Post";
        public const string CancelPostButtonText = "Cancel Post";
        public static SolidColorBrush AvocadoGreen = new SolidColorBrush(Color.FromArgb(0xFF, 0x62, 0x94, 0x3C));
        public ObservableCollection<string> RecurrenceTypes = new ObservableCollection<string>() { "none", "daily", "weekly", "weekdays", "monthly", "yearly" };
        
        #endregion

        #region Static Members

        public static InputDialog CaptionDialog;
        public static TileUpdater LiveTileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
        public static ToastNotifier LiveToastNotifier = ToastNotificationManager.CreateToastNotifier();

        #endregion

        public Activities()
        {
            DispatcherHelper.Initialize();
            
            // Set up the Caption dialog. This will be re-used frequently 
            CaptionDialog = new InputDialog();
            CaptionDialog.BackgroundStripeBrush = AvocadoGreen;
            CaptionDialog.Background = AvocadoGreen;
            CaptionDialog.BorderBrush = new SolidColorBrush(Colors.Transparent);
            CaptionDialog.AcceptButton = PostButtonText;
            CaptionDialog.CancelButton = CancelPostButtonText;            
        }
        public AuthClient AuthClient { get; set; }

        private Couple couple;
        public Couple Couple 
        { 
            get 
            { 
                return couple; 
            } 
            set 
            { 
                couple = value; 
                RaisePropertyChanged("Couple"); 
                RaisePropertyChanged("NewMessagePrompt"); 
            } 
        }

        #region Messages

        private string newMessage;
        public string NewMessage 
        { 
            get 
            { 
                return newMessage; 
            } 
            set 
            { 
                newMessage = value; 
                RaisePropertyChanged("NewMessage"); 
            } 
        }

        public string NewMessagePrompt
        {
            get
            {
                if (Couple == null)
                {
                    return string.Empty;
                }
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
                    var clone = NewMessage;
                    var task = ThreadPool.RunAsync(t =>
                    {
                        AuthClient.SendMessage(clone);
                        
                    });
                    task.Completed = RunOnComplete(Update);

                }
                NewMessage = "";
            }
        }
        public ICommand NewMessageKeyDownCommand
        {
            get
            {
                return new RelayCommand<KeyRoutedEventArgs>(args => { NewMessageKeyDown(args); });
            }
        }

        #endregion

        public void Logout()
        {

        }

        public ICommand LogoutCommand
        {
            get
            {
                return new RelayCommand(() => { Logout(); });
            }
        }

        private bool isLoading;
        public bool IsLoading 
        { 
            get 
            { 
                return isLoading; 
            } set 
            { 
                isLoading = value; 
                RaisePropertyChanged("IsLoading"); 
            } 
        }

        #region ListItemActions

        private List<AvoList> listModelList;
        public List<AvoList> ListModelList
        {
            get
            {
                return listModelList;
            }
            set
            {
                listModelList = value;
                RaisePropertyChanged("ListModelList");
            }
        }

        private AvoList selectedListModel;
        public AvoList SelectedListModel
        {
            get
            {
                if (selectedListModel == null && ListModelList != null && ListModelList.Count > 0)
                {
                    SelectedListModel = ListModelList.First();
                }
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

                //wat
                selectedListModel.Items.CollectionChanged -= new NotifyCollectionChangedEventHandler((t, y) => ListItemDrop(t, y));
                selectedListModel.Items.CollectionChanged += new NotifyCollectionChangedEventHandler((t, y) => ListItemDrop(t, y));

                RaisePropertyChanged("SelectedListModel");
            }
        }

        private object ListItemDrop(object t, NotifyCollectionChangedEventArgs y)
        {
            if (y.Action == NotifyCollectionChangedAction.Add)
            {
                EditListItem(((ObservableCollection<AvoListItem>)t)[y.NewStartingIndex], false, y.NewStartingIndex);
            }
            return t;
        }

        public void EditListItem(AvoListItem listItem, bool delete = false, int index = -1)
        {
            if (string.IsNullOrEmpty(listItem.ListId))
            {
                return;
            }
            IsLoading = true;
            var parentList = (from l in ListModelList
                         where l.Id == listItem.ListId
                         select l).First();

            if (index == -1)
            {
                index = GetListItemIndex(parentList, listItem.Complete, listItem.Important);
                parentList.Items.Move(parentList.Items.IndexOf(listItem), index);
                SelectedListModel.Items = parentList.Items;
            }
            if (delete)
            {
                parentList.Items.Remove(listItem);
            }
            var task = ThreadPool.RunAsync(t => AuthClient.EditListItem(listItem, index, delete));
            task.Completed = RunOnComplete(() => { IsLoading = false; });
        }

        public ICommand EditListItemCommand
        {
            get
            {
                return new RelayCommand<AvoListItem>(param => EditListItem(param));
            }
        }

        public void ToggleListItemImportant(AvoListItem listItem)
        {
            listItem.Important = !listItem.Important;
            EditListItem(listItem);
        }

        public ICommand ToggleListItemImportantCommand
        {
            get
            {
                return new RelayCommand<AvoListItem>(param => { ToggleListItemImportant(param); });
            }
        }

        public ICommand DeleteListItemCommand
        {
            get
            {
                return new RelayCommand<AvoListItem>(param => EditListItem(param, true));
            }
        }

        private string newListItemText;
        public string NewListItemText
        {
            get
            {
                return newListItemText;
            }
            set
            {
                newListItemText = value;
                RaisePropertyChanged("NewListItemText");
            }
        }

        public void NewListItemKeyDown(KeyRoutedEventArgs args)
        {
            if (args.Key == VirtualKey.Enter)
            {
                if (!string.IsNullOrEmpty(NewListItemText))
                {
                    IsLoading = true;
                    var index = GetListItemIndex(SelectedListModel, false, false);
                    var newItem = new AvoListItem() { Text = NewListItemText, Id = "-1", ListId = SelectedListModel.Id };
                    SelectedListModel.Items.Insert(index, newItem);
                    var clone = NewListItemText;
                    var finalItem = newItem;
                    var task = ThreadPool.RunAsync(t =>
                    {
                        finalItem = AuthClient.CreateListItem(clone, SelectedListModel.Id, index);
                        finalItem.ListId = newItem.ListId;
                    });

                    task.Completed = RunOnComplete(() => {
                        var i = SelectedListModel.Items.IndexOf(newItem);
                        if (i >= 0)
                        {
                            SelectedListModel.Items[i] = finalItem;
                        }
                        IsLoading = false; 
                    });

                }
                NewListItemText = "";
            }
        }

        public ICommand NewListItemKeyDownCommand
        {
            get
            {
                return new RelayCommand<KeyRoutedEventArgs>(param => NewListItemKeyDown(param));
            }
        }

        public int GetListItemIndex(AvoList list, bool complete, bool important)
        {
            if (important && !complete) // important and not complete, just add it to the beginning of the list
            {
                return 0;
            }
            else if(!important && !complete) // not important and not complete, add it after all important items
            {
                return (from l in list.Items
                        where l.Important && !l.Complete
                        select l).Count();
            } 
            else // any completed item can be added moved to the end of all uncompleted items 
            {
                return (from l in list.Items
                        where !l.Complete
                        select l).Count();
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
                RaisePropertyChanged("IsMediaTabActive");
                RaisePropertyChanged("IsListTabActive");
                RaisePropertyChanged("IsCalendarTabActive");
                RaisePropertyChanged("IsPreferenceTabActive");

                if (value == TabType.Calendar)
                {
                    var firstItem = (from e in CalendarItems
                                    select e).FirstOrDefault();
                    if (firstItem == null)
                    {
                        SelectedDate = DateTime.Today;
                    }
                    else
                    {
                        SelectedCalendarItem = firstItem;
                    }
                }
            }
        }
        
        public bool IsListTabActive
        {
            get
            {
                return SelectedTab == TabType.Lists;
            }
        }
        
        public bool IsMediaTabActive
        {
            get
            {
                return SelectedTab == TabType.Media;
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

        public ICommand SetMediaTabActiveCommand
        {
            get
            {
                return new RelayCommand(() => { SetTabActive(TabType.Media); });
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

        #region Calendar

        private ObservableCollection<CalendarItem> calendarItems;
        public ObservableCollection<CalendarItem> CalendarItems
        {
            get 
            { 
                return calendarItems; 
            }
            set 
            { 
                calendarItems = value;
                // clear all notifications, then update them.
                foreach (var item in LiveToastNotifier.GetScheduledToastNotifications())
                {
                    LiveToastNotifier.RemoveFromSchedule(item);
                }
                foreach (var item in calendarItems)
                {
                    item.ScheduleReminderNotifications(LiveToastNotifier, Couple.CurrentUser.Id);
                }
                RaisePropertyChanged("CalendarItems"); 
            }
        }

        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                selectedDate = value;
                RaisePropertyChanged("SelectedDate");

                // set it and raise property changed manually to prevent circular reference setting
                selectedCalendarItem = (from item in CalendarItems
                                        where item.StartDate == SelectedDate
                                        select item).FirstOrDefault();
                RaisePropertyChanged("SelectedCalendarItem");
            }
        }

        private CalendarItem selectedCalendarItem;
        public CalendarItem SelectedCalendarItem
        {
            get
            {
                return selectedCalendarItem;
            }
            set
            {
                selectedCalendarItem = value;
                RaisePropertyChanged("SelectedCalendarItem");

                // set it and raise property changed manually to prevent circular reference setting
                selectedDate = selectedCalendarItem.StartDate;
                RaisePropertyChanged("SelectedDate");
            }
        }

        public void SelectDate(DateTime date)
        {
            SelectedDate = date;
        }

        public ICommand SelectDateCommand
        {
            get
            {
                return new RelayCommand<DateTime>(d => SelectDate(d));
            }
        }

        private CalendarItem editingCalendarItem;
        public CalendarItem EditingCalendarItem
        {
            get
            {
                return editingCalendarItem;
            }
            set
            {
                editingCalendarItem = value;
                RaisePropertyChanged("EditingCalendarItem");
            }
        }

        private bool isEventEditPaneVisible;
        public bool IsEventEditPaneVisible
        {
            get
            {
                return isEventEditPaneVisible && IsCalendarTabActive;
            }
            set
            {
                isEventEditPaneVisible = value;
                RaisePropertyChanged("IsEventEditPaneVisible");
            }
        }

        public void BeginEditCalendarItem()
        {
            IsEventEditPaneVisible = true;
            EditingCalendarItem = new CalendarItem()
            {
                Title = SelectedCalendarItem.Title,
                Location = SelectedCalendarItem.Location,
                Attending = SelectedCalendarItem.Attending,
                StartTime = SelectedCalendarItem.StartTime,
                EndTime = SelectedCalendarItem.EndTime,
                Description = SelectedCalendarItem.Description,
                Id = SelectedCalendarItem.Id,
                StartDateTime = SelectedCalendarItem.StartDateTime
            };
        }

        public void BeginNewCalendarItem()
        {
            IsEventEditPaneVisible = true;
            EditingCalendarItem = new CalendarItem() { Id = "-1" };
        }

        public ICommand EditEventCommand
        {
            get
            {
                return new RelayCommand(() => BeginEditCalendarItem());
            }
        }

        public ICommand NewEventCommand
        {
            get
            {
                return new RelayCommand(() => BeginNewCalendarItem());
            }
        }

        public void CancelEditEvent()
        {
            IsEventEditPaneVisible = false;
        }

        public ICommand CancelEditEventCommand
        {
            get
            {
                return new RelayCommand(() => CancelEditEvent());
            }
        }

        #endregion

        #region Media

        private List<Media> mediaList;
        public List<Media> MediaList
        {
            get
            {
                return mediaList;
            }
            set
            {
                mediaList = value;
                RaisePropertyChanged("MediaList");
            }
        }

        public Media selectedMediaListItem;
        public Media SelectedMediaListItem
        {
            get
            {
                return selectedMediaListItem;
            }
            set
            {
                selectedMediaListItem = value;
                RaisePropertyChanged("SelectedMediaListItem");
            }
        }

        public void AddPhoto()
        {
            // We need to run this async, but on the UI thread (... yeah, me either)
            DispatcherHelper.CheckBeginInvokeOnUI(() => { PickPhoto(); });
        }

        public async void PickPhoto()
        {
            IsLoading = true;
            FileOpenPicker open = new FileOpenPicker();
            open.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            open.ViewMode = PickerViewMode.Thumbnail;
            
            // Filter to include a sample subset of file types
            open.FileTypeFilter.Clear();
            open.FileTypeFilter.Add(".bmp");
            open.FileTypeFilter.Add(".png");
            open.FileTypeFilter.Add(".jpeg");
            open.FileTypeFilter.Add(".jpg");

            // Open a stream for the selected file
            StorageFile file = await open.PickSingleFileAsync();

            // Ensure a file was selected
            
            if (file == null)
            {
                IsLoading = false;
                return;
            }
            var stream = await file.OpenStreamForReadAsync();
                
            
            var result = await CaptionDialog.ShowAsync("Caption?", "Totally optional", PostButtonText, CancelPostButtonText);
            string captionResult = CaptionDialog.InputText;
            CaptionDialog.InputText = string.Empty;
            if (result == CancelPostButtonText) 
            {
                IsLoading = false;
                return;
            }
            var task = ThreadPool.RunAsync(t => AuthClient.UploadPhoto(file.Name, file.ContentType, stream, captionResult));
            task.Completed = RunOnComplete(Update);
            //AuthClient.UploadPhoto(file.Name, file.ContentType, stream);
            // ON UI Thread - we're here - do a confirm, then start the loading indicator, call an async operation to upload photo
           
        }

        public ICommand AddPhotoCommand
        {
            get
            {
                return new RelayCommand(() => AddPhoto());
            }
        }

        #endregion

        #region Activities

        private List<Activity> activityList;
        public List<Activity> ActivityList
        {
            get
            {
                return activityList;
            }
            set
            {
                activityList = value;
                RaisePropertyChanged("ActivityList");
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

        public void ClearListActivities()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(ConfirmClearListActivities);
        }

        public async void ConfirmClearListActivities()
        {
            var messageBox = MessageBoxService.Default;

            var result = await messageBox.ShowAsync("Are you sure you want to clear all activities about lists?", "Are you sure?", GenericMessageBoxButton.OkCancel);
            if (result == GenericMessageBoxResult.Cancel)
            {
                return;
            }

            IsLoading = true;
            var task = ThreadPool.RunAsync(t => ClearListActivitesAsync());

            task.Completed = RunOnComplete(Update);
        }

        public async void ClearListActivitesAsync()
        {
            var listActivities = (from a in ActivityList
                                  where a.IsList == true
                                  select a).ToList();
            foreach (var listActivity in listActivities)
            {
                AuthClient.DeleteActivity(listActivity.Id);
            }
        }

        public ICommand ClearListActivitiesCommand
        {
            get
            {
                return new RelayCommand(() => { DispatcherHelper.CheckBeginInvokeOnUI(ClearListActivities); });
            }
        }

        public void DeleteActivity()
        {
            IsLoading = true;
            var idToDelete = SelectedActivity.Id;
            var task = ThreadPool.RunAsync(t => { AuthClient.DeleteActivity(idToDelete); });
            task.Completed = RunOnComplete(Update);
        }

        public ICommand DeleteActivityCommand
        {
            get
            {
                return new RelayCommand(() => DeleteActivity());
            }
        }

        #endregion

        public AsyncActionCompletedHandler RunOnComplete(Action method)
        {
            return new AsyncActionCompletedHandler((IAsyncAction source, AsyncStatus status) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() => { method(); });
            });
        }

        #region Updating

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(() => Update());
            }
        }

        public async Task UpdateData()
        {
            couple = AuthClient.GetUsers();
            activityList = AuthClient.GetActivities();
            listModelList = AuthClient.GetListModelList();
            calendarItems = AuthClient.GetCalendar();
            mediaList = AuthClient.GetMedia();
            return;
        }

        public void ProcessUpdate()
        {
            Couple = couple;
            ActivityList = activityList;
            ListModelList = listModelList;
            CalendarItems = calendarItems;
            MediaList = mediaList;
            foreach (var activity in ActivityList)
            {
                activity.User = activity.UserId == Couple.CurrentUser.Id ? Couple.CurrentUser : Couple.OtherUser;
            }
            ActivityList.Reverse();

            var tab = SelectedTab;
            var selectedList = SelectedListModel;

            SelectedActivity = ActivityList.First();
            SelectedListModel = selectedList;

            //restore the selected tab in case restoring the selected list caused this to change
            SelectedTab = tab;
            IsLoading = false;
            UpdateLiveTile();

        }

        public void Update()
        {
            IsLoading = true;
            var task = ThreadPool.RunAsync(t => UpdateData());

            task.Completed = RunOnComplete(ProcessUpdate);
        }

        public void UpdateLiveTile()
        {
            LiveTileUpdater.Clear();
            var uri = AuthClient.GetTileUpdateUri(Couple.CurrentUser.Id);
            LiveTileUpdater.StartPeriodicUpdate(uri, PeriodicUpdateRecurrence.HalfHour);
        }

        #endregion
    }
}

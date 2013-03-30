using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avocado.Models;
using Avocado.ViewModel;
using MetroMVVM;
using MetroMVVM.NotificationsExtensions.ToastContent;
using Windows.UI.Notifications;

namespace Avocado.ViewModels
{
    public class CalendarItem : ObservableObject
    {
        public string Id;
        public string userId;
        public double StartTime;
        public double EndTime;
        public double TimeCreated;
        public double TimeUpdated;

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get
            {
                if (startDate == null || startDate == DateTime.MinValue)
                {
                    StartDate = (Utilities.UnixTimeStampToDateTime(StartTime)).Date;
                }
                return startDate;
            }
            set
            {
                startDate = value;
                RaisePropertyChanged("StartDate");
            }
        }

        private DateTime startDateTime;
        public DateTime StartDateTime
        {
            get
            {
                if (startDateTime == null || startDateTime == DateTime.MinValue)
                {
                    StartDateTime = Utilities.UnixTimeStampToDateTime(StartTime);
                }
                return startDateTime;
            }
            set
            {
                startDateTime = value;
                RaisePropertyChanged("StartDateTime");
            }
        }

        private DateTime endDateTime;
        public DateTime EndDateTime
        {
            get
            {
                if (endDateTime == null || endDateTime == DateTime.MinValue)
                {
                    endDateTime = Utilities.UnixTimeStampToDateTime(EndTime);
                }
                return endDateTime;
            }
        }

        private DateTime dateTimeCreated;
        public DateTime DateTimeCreated
        {
            get
            {
                if (dateTimeCreated == null || dateTimeCreated == DateTime.MinValue)
                {
                    dateTimeCreated = Utilities.UnixTimeStampToDateTime(TimeCreated);
                }
                return dateTimeCreated;
            }
        }

        private DateTime dateTimeUpdated;
        public DateTime DateTimeUpdated
        {
            get
            {
                if (dateTimeUpdated == null || dateTimeUpdated == DateTime.MinValue)
                {
                    dateTimeUpdated = Utilities.UnixTimeStampToDateTime(TimeUpdated);
                }
                return dateTimeUpdated;
            }
        }

        private string location;
        public string Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
                RaisePropertyChanged("Location");
            }
        }

        private List<string> attending;
        public List<string> Attending
        {
            get
            {
                return attending;
            }
            set
            {
                attending = value;
                RaisePropertyChanged("Attending");
            }
        }

        public string DateString
        {
            get
            {
                var dateString = string.Empty;
                if (StartDateTime != null)
                {
                    dateString += StartDateTime.ToString("MMM d") + StartDateTime.ToString(" @ h:mmt").ToLower();
                    if (EndDateTime != null)
                    {
                        dateString += " - " + EndDateTime.ToString("h:mmt").ToLower();
                    }
                }
                return dateString;
            }
        }


        public ObservableCollection<Reminder> Reminders;

        private string recurrenceType;
        public string RecurrenceType
        {
            get
            {
                return recurrenceType;
            }
            set
            {
                recurrenceType = value;
                RaisePropertyChanged("RecurrenceType");
            }
        }

        public string indicator;
        public string Indicator 
        { 
            get 
            { 
                return indicator; 
            } 
            set { 
                indicator = value; 
                RaisePropertyChanged("Indicator"); 
            } 
        }

        public CalendarItem()
        {
            Indicator = "•";
        }

        public void ScheduleReminderNotifications(ToastNotifier notifier)
        {
            foreach (var reminder in Reminders)
            {
                var toast = ToastContentFactory.CreateToastText04();
                toast.TextHeading.Text = Title;
                toast.TextBody2.Text = Location;

                var reminderTime = Utilities.UnixTimeStampToDateTime(StartTime - reminder.Interval);
                var timeString = IntervalToString(StartDateTime - reminderTime);
                toast.TextBody1.Text = timeString;

                toast.Duration = ToastDuration.Long;
                toast.Audio.Loop = false;
                toast.Audio.Content = ToastAudioContent.Reminder;

                if (reminderTime < DateTime.Now)
                {
                    continue;
                }
                var scheduled = new ScheduledToastNotification(toast.CreateNotification().Content, reminderTime);
                
                notifier.AddToSchedule(scheduled);
            }
        }

        private string IntervalToString(TimeSpan interval)
        { 
            if ((int)interval.TotalDays > 1)
            {
                return DateString;
            }
            else if ((int)interval.TotalDays == 1 || (int)interval.TotalHours > 12)
            {
                return string.Format("tomorrow at {0}:{1}", StartDate.Hour, StartDate.Minute);
            }
            else if ((int)interval.TotalHours > 1)
            {
                return string.Format("in {0} hours", (int)interval.TotalHours);
            }
            else if ((int)interval.TotalHours == 1)
            {
                return string.Format("in {0} hour", (int)interval.TotalHours);
            }
            else if ((int)interval.TotalMinutes > 1)
            {
                return string.Format("in {0} minutes", (int)interval.TotalMinutes);
            }
            else
            {
                return string.Format("Now");
            }
        }
    }
}

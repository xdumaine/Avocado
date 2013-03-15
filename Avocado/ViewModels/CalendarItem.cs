using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avocado.Models;
using Avocado.ViewModel;
using MetroMVVM;

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
    }
}

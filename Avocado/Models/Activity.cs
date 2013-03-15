using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Avocado.ViewModel;
using MetroMVVM;

namespace Avocado.Models
{
    public class Activity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public double TimeCreated { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public string EventString
        {
            get
            {
                switch (Type)
                {
                    case "message":
                        return string.Format("sent a message");
                    case "list":
                        return string.Format("{0}ed the list '{1}'", Action, Data.Name);
                    case "kiss":
                        return "sent you a kiss!";
                    case "hug":
                        return "hugged you!";
                    case "photo":
                        return "posted a photo";
                    case "event":
                        return string.Format("{0}ed the event: {1}", Action, Data.Name);
                    default:
                        return "Did something I don't know about - " + Type;
                }
            }
        }

        #region TypeToBoolean
        public bool IsImage
        {
            get
            {
                return Type == "photo";
            }
        }
        public bool IsList
        {
            get
            {
                return Type == "list";
            }
        }
        public bool IsMessage
        {
            get
            {
                return Type == "message";
            }
        }
        public bool IsEvent
        {
            get
            {
                return Type == "event";
            }
        }
        #endregion

        public DateTime date;
        public DateTime Date
        {
            get
            {
                if (date == null || date == DateTime.MinValue)
                {
                    date = Utilities.UnixTimeStampToDateTime(TimeCreated);
                }
                return date;
            }
        }

        public string dateString;
        public string DateString
        {
            get
            {
                if(string.IsNullOrEmpty(dateString))
                {
                    var ts = DateTime.Now - Date;
                    if ((int)ts.TotalDays > 1)
                    {
                        dateString = string.Format("{0} days ago", (int)ts.TotalDays);
                    }
                    else if ((int)ts.TotalDays == 1 || date.Date == DateTime.Now.AddDays(-1).Date)
                    {
                        dateString = string.Format("yesterday");
                    }
                    else if ((int)ts.TotalHours > 1)
                    {
                        dateString = string.Format("{0} hours ago", (int)ts.TotalHours);
                    }
                    else if ((int)ts.TotalMinutes > 1)
                    {
                        dateString = string.Format("{0} minutes ago", (int)ts.TotalMinutes);
                    }
                    else
                    {
                        dateString = string.Format("just now");
                    }
                }
                return dateString;
            }
        }

        public ActivityData Data { get; set; }

        public User User { get; set; }
    }
}

﻿using System;
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

    // Comments above each item specify which activity type they are used in
    public class ActivityData
    {
        // List; Photo; Media; Activity;
        public string Id { get; set; }

        // Message; 
        public string Text { get; set; }

        // Photo;
        public string Url { get; set; }

        // Name; List;
        public string Name { get; set; }

        // Photo;
        public string Caption { get; set; }

        // Photo;
        public PhotoInfo Info { get; set; }
        
        // Kiss; 
        public ImageUrlCollection ImageUrls { get; set; }

        // Kiss;
        public List<Kiss> Kisses { get; set; }

        // User; 
        public string Attribute { get; set; }
    }

    

    

    

    

    

    

    



    #region PreviousCode
    //[KnownType(typeof(Hug))]
    //[KnownType(typeof(Kiss))]
    //[KnownType(typeof(Message))]
    //[KnownType(typeof(ListActivity))]
    //public abstract class Activity
    //{
    //    public string Id { get; set; }
    //    public string UserId { get; set; }
    //    public long TimeCreated { get; set; }
    //    public string Type { get; set; }
    //    public string Action { get; set; }

    //    public string EventString
    //    {
    //        get
    //        {
    //            return GetEventString();
    //        }
    //    }

    //    public abstract string GetEventString();

    //    public DateTime DateTimeCreated { get { return new DateTime(TimeCreated); } }
    //}

    //public class Message : Activity
    //{
    //    public string Text { get; set; }
    //    public override string GetEventString()
    //    {
    //        return string.Format("said \"{0}\"", Text);
    //    }
    //}

    //public class ListActivity : Activity
    //{
    //    public string Id { get; set; }
    //    public string Name { get; set; }
    //    public override string GetEventString()
    //    {
    //        return string.Format("{0} list {1}", Action, Name);
    //    }
    //}

    //public class Hug : Activity
    //{
    //    public override string GetEventString()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class Photo : Activity
    //{
    //    public string Caption { get; set; }
    //    public string Url { get; set; }
    //    public string UrlTiny { get; set; }
    //    public string UrlSmall { get; set; }
    //    public string UrlMedium { get; set; }
    //    public string UrlLarge { get; set; }
    //    public PhotoDetail Info { get; set; }

    //    public override string GetEventString()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class PhotoDetail
    //{
    //    public int Width { get; set; }
    //    public int Height { get; set; }
    //    public double AspectRatio { get; set; }
    //    public string format { get; set; }
    //    public int Size { get; set; }
    //}

    //public class Kiss : Activity
    //{
    //    public Collection<KissDetail> Kisses { get; set; }
    //    public string ImageUrlSmall { get; set; }
    //    public string ImageUrlMedium { get; set; }
    //    public string ImageUrlLarge { get; set; }

    //    public override string GetEventString()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class KissDetail
    //{
    //    public double x { get; set; }
    //    public double y { get; set; }
    //    public double rotation { get; set; }
    //}
    #endregion

}
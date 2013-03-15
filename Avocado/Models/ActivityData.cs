using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avocado.Models
{
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
}

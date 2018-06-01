using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace MyTeam.Models
{
    public class RssModel
    {
        public ImageSource SiteLogo { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime PublishedDatetime { get; set; }

        //Η στήλη χρησιμοποιείται μόνο για να κάνουμε το grouping χωρίς να πειράξουμε την ταξινόμηση
        public string GroupingDate => GroupTextByDate(PublishedDatetime);


        public string GroupTextByDate(DateTime publishedDateTime)
        {
            //Ανάλογα με την ημερομηνία που θα πάρουμε επιστρέφουμςε μια από τις τρεις περιπτώσεις
            int daysDiff = (DateTime.Now.Date - publishedDateTime.Date).Days;

            switch (daysDiff)
            {
                case 0:
                    return " Σημερινά άρθρα - " + publishedDateTime.ToString("dd MMM yyyy");
                case 1:
                    return " Χθεσινά άρθρα - " + publishedDateTime.ToString("dd MMM yyyy");
                default:
                    return "Παλαιότερα άρθρα";
            }



        }



    }

    
}

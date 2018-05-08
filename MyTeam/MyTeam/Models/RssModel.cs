using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MyTeam.Models
{
    public class RssModel
    {
        public ImageSource SiteLogo { get; set; }
        public string Title { get; set; }
        //public string Description { get; set; }
        public string Url { get; set; }
        public DateTime PublishedDatetime { get; set; }
    }
}

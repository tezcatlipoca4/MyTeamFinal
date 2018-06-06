using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MyTeam
{
    public partial class NewsPapersPage : ContentPage
    {
        public NewsPapersPage()
        {
            InitializeComponent();

			var htmlSource = new HtmlWebViewSource
            {
                Html = @"<html><body><p><iframe src='https://www.frontpages.gr/ticker.php?category=2&c=F2F2F2&w=350&h=480&t=1&e=0' width='350' height='480' scrolling=no marginwidth=0 marginheight=0 frameborder=0 border=0 style='border:0;margin:0;padding:0;'></iframe></p></body></html>"
            };

            Browser.Source = htmlSource;
        }
    }
}




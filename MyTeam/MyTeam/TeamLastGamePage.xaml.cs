using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyTeam
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TeamLastGamePage : ContentPage
	{
        public string activeTeam;

        public TeamLastGamePage ()
		{
			InitializeComponent ();

            
            switch (SettingsPage.TeamChosen)
            {
                case "paok":
                    activeTeam = "paok-thessaloniki-fc-191542";
                    break;

                case "aek":
                    activeTeam = "aek-athens-179288";
                    break;

                case "osfp":
                    activeTeam = "olympiacos-191194";
                    break;

                case "panathinaikos":
                    activeTeam = "panathinaikos-191512";
                    break;                
            }

            var htmlSource = new HtmlWebViewSource
            {
                Html = @"<html><body>                            
                         <p><iframe frameborder='0'  " +
                         "scrolling='no' width='300' height='300' " +
                         "src='https://www.fctables.com/teams/" + 
                         activeTeam + 
                         "/iframe/?type=team-last-match&lang_id=23&country=87&template=71&team=191542&timezone=Europe/Bucharest&time=24&width=300&height=300&font=Verdana&fs=12&lh=22&bg=FFFFFF&fc=333333&logo=1&tlink=0&scfs=22&scfc=333333&scb=1&sclg=1&teamls=80&sh=1&hfb=1&hbc=3bafda&hfc=FFFFFF'></iframe></p></body></html>"
            };

            Browser.Source = htmlSource;
        }
  
    }
}
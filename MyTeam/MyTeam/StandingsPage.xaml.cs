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
	public partial class StandingsPage : ContentPage
	{
		public StandingsPage ()
		{
			InitializeComponent ();
                        
            var htmlSource = new HtmlWebViewSource
            {
                Html = @"<html><body>
                            
                            <p><iframe frameborder='0'  " +
                            "scrolling='no' width='520' height='700' " +
                            "src='https://www.fctables.com/greece/super-league/iframe/?type=table&lang_id=23&country=87&template=71&team="+SettingsPage.TeamChosenFcTablesNumberOnly+"&timezone=Europe/Bucharest&time=24&po=1&ma=1&wi=1&dr=1&los=1&gf=1&ga=1&gd=1&pts=1&ng=0&form=1&width=520&height=700&font=Verdana&fs=12&lh=22&bg=FFFFFF&fc=333333&logo=1&tlink=0&ths=1&thb=1&thba=FFFFFF&thc=000000&bc=dddddd&hob=f5f5f5&hobc=ebe7e7&lc=333333&sh=1&hfb=1&hbc=3bafda&hfc=FFFFFF'></iframe></p></body></html>"
            };

            Browser.Source = htmlSource;
        }
	}
}
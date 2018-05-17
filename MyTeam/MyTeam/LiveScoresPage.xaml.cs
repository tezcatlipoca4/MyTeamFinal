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
	public partial class LiveScoresPage : ContentPage
	{
		public LiveScoresPage ()
		{
			InitializeComponent ();

            var htmlSource = new HtmlWebViewSource
            {
                Html = @"<html><body>
                            
                            <p><iframe frameborder='0'  " +
                            "scrolling='no' width='350' height='440' " +
                            "src='https://www.fctables.com/greece/super-league/iframe/?type=league-scores&lang_id=23&country=87&template=71&team=&timezone=Europe/Bucharest&time=24&width=350&height=440&font=Verdana&fs=12&lh=22&bg=FFFFFF&fc=333333&logo=1&tlink=0&scoreb=0a0a0a&scorefc=FFFFFF&sgdcoreb=8f8d8d&sgdcorefc=FFFFFF&sh=1&hfb=1&hbc=3bafda&hfc=FFFFFF'></iframe></p></body></html>"
            };

            Browser.Source = htmlSource;
        }
	}
}
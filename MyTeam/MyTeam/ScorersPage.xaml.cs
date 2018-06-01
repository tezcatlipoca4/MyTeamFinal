using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MyTeam
{
    public partial class ScorersPage : ContentPage
    {
        public ScorersPage()
        {
            InitializeComponent();

			var htmlSource = new HtmlWebViewSource
            {
                Html = @"<html><body>                            
                         <p><iframe frameborder='0'  " +
					"scrolling='no' width='360' height='500' " +
					"src='https://www.fctables.com/greece/super-league/iframe=/?type=top-scorers&lang_id=23&country=87&template=71&team=&timezone=Europe/Athens&time=24&limit=10&ppo=1&pte=1&pgo=1&pma=1&pas=0&ppe=1&width=520&height=700&font=Verdana&fs=12&lh=22&bg=FFFFFF&fc=333333&logo=1&tlink=0&ths=1&thb=1&thba=FFFFFF&thc=000000&bc=dddddd&hob=f5f5f5&hobc=ebe7e7&lc=333333&sh=1&hfb=1&hbc=000000&hfc=FFFFFF'></iframe></p></body></html>"
            };

            Browser.Source = htmlSource;
        }
    }
}

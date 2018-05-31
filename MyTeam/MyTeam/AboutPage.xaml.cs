using System;
using System.IO;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyTeam
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
		public string AppVersionNumber;

        public AboutPage()
        {
            InitializeComponent();

			// Get version number
            AppVersionNumber = DependencyService.Get<App.IGetVersionNumber>().GetVersion();

            var assembly = typeof(AboutPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("MyTeam.Assets.HtmlFiles.About.html");

            StreamReader reader = new StreamReader(stream);
            string htmlString = reader.ReadToEnd();

            HtmlWebViewSource html = new HtmlWebViewSource();
            html.Html = htmlString;

            webView.Source = html;
            //Το κουμπί θα εμφανιστεί μόνο την πρώτη φορά που θα τρέξει η εφαρμογή

            // Version Number
			versionNumber.Text = "Έκδοση: " + AppVersionNumber;

            //if (SettingsPage.TeamChosen == string.Empty)
			if(App.TutorialMode)
                AgreeButton.IsVisible = true;
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new SettingsPage();
        }
    }
}
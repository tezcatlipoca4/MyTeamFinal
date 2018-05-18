using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyTeam
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            List<string> list = new List<string> { "Κεντρική Σελίδα", "Ρυθμισεις", "Προηγούμενος αγώνας", "Επόμενος αγώνας", "Βαθμολογία", "Live Score", "Σχετικά με την Konstraction" };
            listView.ItemsSource = list;

            //Todo: Έλεγχος αν είναι η πρώτη φορά που τρέχει η εφαρμογή να πηγαίνει στις ρυθμίσεις με την επιλογή για feed απενεργοποιημένη
            //Διαφορετικά πηγαίνει κανονικά στο feed

            //Για την αρχική εκκίνηση της εφαρμογής
            navigationDrawer.ContentView = new RssFeedPage().Content;
            backButton.IsVisible = false;



        }

        private void HamburgerButton_OnClicked(object sender, EventArgs e)
        {
            navigationDrawer.ToggleDrawer();
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            navigationDrawer.ContentView = new RssFeedPage().Content;
            backButton.IsVisible = false;
        }


        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            //Με την επιλογή ενός αντικειμένου από το μενου
            switch (e.SelectedItem.ToString())
            {
                case "Κεντρική Σελίδα":
                    navigationDrawer.ContentView = new RssFeedPage().Content;
                    backButton.IsVisible = false;
                    break;

                case "Ρυθμισεις":
                    navigationDrawer.ContentView = new SettingsPage().Content;
                    backButton.IsVisible = true;
                    break;

                case "Σχετικά με την Konstraction":
                    backButton.IsVisible = true;
                    break;

                case "Βαθμολογία":
                    navigationDrawer.ContentView = new StandingsPage().Content;
                    backButton.IsVisible = true;
                    break;

                case "Προηγούμενος αγώνας":
                    navigationDrawer.ContentView = new TeamLastGamePage().Content;
                    backButton.IsVisible = true;
                    break;

                case "Επόμενος αγώνας":
                    navigationDrawer.ContentView = new TeamNextMatchPage().Content;
                    backButton.IsVisible = true;
                    break;

                case "Live Score":
                    navigationDrawer.ContentView = new LiveScoresPage().Content;
                    backButton.IsVisible = true;
                    break;
            }

            //Αλλάζουμε την επικεφαλίδα ανάλογα με την επιλογή
            headerLabel.Text = e.SelectedItem.ToString();
            //κλέινουμε το menu
            navigationDrawer.ToggleDrawer();
        }
    }
}
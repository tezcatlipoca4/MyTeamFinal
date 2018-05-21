using System;
using System.Collections.Generic;
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

            //Για την αρχική εκκίνηση της εφαρμογής
            navigationDrawer.ContentView = new RssFeedPage().Content;
            backButton.IsVisible = false;

            List<string> menuList = new List<string>
            {
                "Ειδήσεις Ομάδας",
                "Προηγούμενος αγώνας",
                "Επόμενος αγώνας",
                "Βαθμολογία",
                "Live Score",
                "Ρυθμισεις",
                "Σχετικά με την εφαρμογή"
            };
            listView.ItemsSource = menuList;

            // Έλεγχος εάν η συσκευή Android διαθέτει Software Navigation Keys
            if (Device.RuntimePlatform == Device.Android)
            {
                bool hasNavigationBar = DependencyService.Get<App.IHasHardwareKeys>().IsNavigationBarAvailable();
                screenHeight.Height = hasNavigationBar ? 450 : 500;
            }
            else { screenHeight.Height = 500; } // Fixed height εάν είναι iOS. Θα δούμε την συμπεριφορά του αργότερα

            //Todo: Έλεγχος αν είναι η πρώτη φορά που τρέχει η εφαρμογή να πηγαίνει στις ρυθμίσεις με την επιλογή για feed απενεργοποιημένη
            //Διαφορετικά πηγαίνει κανονικά στο feed

        }

        private void HamburgerButton_OnClicked(object sender, EventArgs e)
        {
            navigationDrawer.ToggleDrawer();
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            listView.SelectedItem = "Ειδήσεις Ομάδας";
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //Με την επιλογή ενός αντικειμένου από το μενου
            switch (e.SelectedItem.ToString())
            {
                case "Ειδήσεις Ομάδας":

                    navigationDrawer.ContentView = new RssFeedPage().Content;
                    backButton.IsVisible = false;
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
                case "Ρυθμισεις":
                    navigationDrawer.ContentView = new SettingsPage().Content;
                    backButton.IsVisible = true;
                    break;

                case "Σχετικά με την εφαρμογή":
                    navigationDrawer.ContentView = new AboutPage().Content;
                    backButton.IsVisible = true;
                    break;
            }

            //Αλλάζουμε την επικεφαλίδα ανάλογα με την επιλογή
            headerLabel.Text = e.SelectedItem.ToString();
            //κλέινουμε το menu
            if (navigationDrawer.IsOpen) navigationDrawer.ToggleDrawer();
        }
    }
}
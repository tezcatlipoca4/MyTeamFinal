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

            // Έλεγχος εάν η συσκευή Android διαθέτει Software Navigation Keys
            if (Device.RuntimePlatform == Device.Android)
            {
                bool hasNavigationBar = DependencyService.Get<App.IHasHardwareKeys>().IsNavigationBarAvailable();
                screenHeight.Height = hasNavigationBar ? 450 : 500;
            }
            else { screenHeight.Height = 500; } // Fixed height εάν είναι iOS. Θα δούμε την συμπεριφορά του αργότερα

            // Έλεγχος σύνδεσης κατά την εκκίνηση
            CheckConnectionAndNavigateToContent(new RssFeedPage().Content);            
            backButton.IsVisible = false; // Hidden by default

            // Δημιουργία sidebar menu
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
            listView.ItemsSource = menuList; // Bind στο ListView
           
            //Todo: Έλεγχος αν είναι η πρώτη φορά που τρέχει η εφαρμογή να πηγαίνει στις ρυθμίσεις με την επιλογή για feed απενεργοποιημένη
            //Διαφορετικά πηγαίνει κανονικά στο feed

        }
        
        // Πάνω αριστερά button
        private void HamburgerButton_OnClicked(object sender, EventArgs e)
        {
            navigationDrawer.ToggleDrawer();
        }

        // Πάνω δεξιά button
        private void BackButton_Clicked(object sender, EventArgs e)
        {
            listView.SelectedItem = "Ειδήσεις Ομάδας";
        }

        // Έλεγχος σύνδεσης στο ίντερνετ και popup error msg
        public void CheckConnectionAndNavigateToContent(View contentView)
        {
            if (App.IsDeviceConnected())
            {
                navigationDrawer.ContentView = contentView;                
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Σφάλμα",
                    "Παρακαλώ, ελέγξτε τη σύνδεσή σας στο ίντερνετ",
                    "ΟΚ");
                });
            }
        }

        // Sidebar menu on item selected actions
        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //Με την επιλογή ενός αντικειμένου από το μενου
            switch (e.SelectedItem.ToString())
            {
                case "Ειδήσεις Ομάδας":
                    CheckConnectionAndNavigateToContent(new RssFeedPage().Content);
                    backButton.IsVisible = false;
                    break;


                case "Βαθμολογία":
                    CheckConnectionAndNavigateToContent(new StandingsPage().Content);                    
                    backButton.IsVisible = true;
                    break;

                case "Προηγούμενος αγώνας":
                    CheckConnectionAndNavigateToContent(new TeamLastGamePage().Content);
                    backButton.IsVisible = true;
                    break;

                case "Επόμενος αγώνας":
                    CheckConnectionAndNavigateToContent(new TeamNextMatchPage().Content);
                    backButton.IsVisible = true;
                    break;

                case "Live Score":
                    CheckConnectionAndNavigateToContent(new LiveScoresPage().Content);
                    backButton.IsVisible = true;
                    break;
                case "Ρυθμισεις":
                    CheckConnectionAndNavigateToContent(new SettingsPage().Content);
                    backButton.IsVisible = true;
                    break;

                case "Σχετικά με την εφαρμογή":
                    CheckConnectionAndNavigateToContent(new AboutPage().Content);
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
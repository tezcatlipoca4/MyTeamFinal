using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyTeam
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        // Ctor
        public MainPage()
        {
            InitializeComponent();

            //Tutorial
            RunTutorial();

            // Έλεγχος εάν η συσκευή Android διαθέτει Software Navigation Keys
            CheckAndroidNavigation();

            // Έλεγχος σύνδεσης κατά την εκκίνηση
            CheckConnectionAndNavigateToContent(new RssFeedPage().Content);

            // Δημιουργία sidebar menu
            List<string> menuList = new List<string>
            {
                "Ειδήσεις Ομάδας",
                "Προηγούμενος αγώνας",
                "Επόμενος αγώνας",
                "Γενικές Ειδήσεις",
                "TV Πρόγραμμα",
                "Βαθμολογία",
                "Σκόρερς",
                "Live Score",
                "Ρυθμισεις",
                "Σχετικά με την εφαρμογή"
            };

            // Bind στο ListView
            listView.ItemsSource = menuList;

        }

        #region Methods
        public void CheckAndroidNavigation()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                bool hasNavigationBar = DependencyService.Get<App.IHasHardwareKeys>().IsNavigationBarAvailable();
                screenHeight.Height = hasNavigationBar ? 450 : 500;
            }
            else { screenHeight.Height = 500; } // Fixed height εάν είναι iOS. Θα δούμε την συμπεριφορά του αργότερα
        }

        public void RunTutorial()
        {
            if (App.TutorialMode)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Συγχαρητήρια!",
                                       "Ρυθμίσατε την εφαρμογή για την ομάδα '" + SettingsPage.TeamLabel + "'!",
                        "ΟΚ!");
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert(SettingsPage.TeamLabel,
                                           "Σύρετε το δάχτυλό σας απο την αριστερή άκρη της οθόνης για να εμφανιστεί το μενού της εφαρμογής.",
                            "ΔΕΙΞΕ ΜΟΥ");
                        navigationDrawer.ToggleDrawer();
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert(SettingsPage.TeamLabel,
                                               "Από εδώ μπορείτε να μεταβείτε σε όλες τις λειτουργίες της εφαρμογής.",
                                "ΤΕΛΟΣ");
                        });
                    });



                });
                App.TutorialMode = false;

            }
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
                    await DisplayAlert("Σφάλμα Σύνδεσης",
                    "Παρακαλώ, ελέγξτε τη σύνδεσή σας στο ίντερνετ",
                    "ΟΚ");
                });
                navigationDrawer.ContentView = new RssFeedPage().Content;
                backButton.Text = "Ξαναδοκιμάστε";
                backButton.TextColor = Color.Red;

            }
        }

        // Sidebar menu on item selected actions
        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            //Με την επιλογή ενός αντικειμένου από το μενου
            switch (e.SelectedItem.ToString())
            {
                case "Ειδήσεις Ομάδας":
                    //Σε περίπτωση που είχαμε πριν ενεργοποιήσει τις γενικές ειδήσεις το ξετσεκάρουμε από εδώ
                    RssFeedPage.GeneralNewsSelected = false;

                    CheckConnectionAndNavigateToContent(new RssFeedPage().Content);
                    backButton.IsVisible = false;
                    break;

                case "Γενικές Ειδήσεις":
                    //Ενεργοποιούμε το flag για τις γενικές ειδήσεις
                    RssFeedPage.GeneralNewsSelected = true;

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

				case "TV Πρόγραμμα":
					CheckConnectionAndNavigateToContent(new TvProgramPage().Content);
					backButton.IsVisible = true;
					break;

                case "Επόμενος αγώνας":
                    CheckConnectionAndNavigateToContent(new TeamNextMatchPage().Content);
                    backButton.IsVisible = true;
                    break;

                case "Σκόρερς":
                    CheckConnectionAndNavigateToContent(new ScorersPage().Content);
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
        #endregion
    }
}
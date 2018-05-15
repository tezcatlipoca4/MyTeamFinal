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

            List<string> list = new List<string> { "Κεντρική Σελίδα", "Ρυθμισεις", "Σχετικά με την Konstraction" };
            listView.ItemsSource = list;

            //Todo: Έλεγχος αν είναι η πρώτη φορά που τρέχει η εφαρμογή να πηγαίνει στις ρυθμίσεις με την επιλογή για feed απενεργοποιημένη
            //Διαφορετικά πηγαίνει κανονικά στο feed

            //Δεν εχουμε ομάδα
            if (SettingsPage.TeamChosen == string.Empty)
            {
                navigationDrawer.ContentView = new SettingsPage().Content;
                headerLabel.Text = "Ρυθμίσεις";

                navigationDrawer.IsEnabled = false;

            }
            else
            {
                navigationDrawer.ContentView = new RssFeedPage().Content;
            }

        }

        private void HamburgerButton_OnClicked(object sender, EventArgs e)
        {
            navigationDrawer.ToggleDrawer();
        }


        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            //Με την επιλογή ενός αντικειμένου από το μενου
            switch (e.SelectedItem.ToString())
            {
                case "Κεντρική Σελίδα":
                    navigationDrawer.ContentView = new RssFeedPage().Content;
                    break;
                case "Ρυθμισεις":
                    navigationDrawer.ContentView = new SettingsPage().Content;
                    break;
                case "Σχετικά με την Konstraction":

                    break;
            }

            //Αλλάζουμε την επικεφαλίδα ανάλογα με την επιλογή
            headerLabel.Text = e.SelectedItem.ToString();
            //κλέινουμε το menu
            navigationDrawer.ToggleDrawer();
        }
    }
}
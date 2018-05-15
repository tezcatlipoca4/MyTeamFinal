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

            List<string> list = new List<string> {"Κεντρική Σελίδα", "Ρυθμισεις", "Σχετικά με την Konstraction"};

            listView.ItemsSource = list;
        }

        private void HamburgerButton_OnClicked(object sender, EventArgs e)
        {
            navigationDrawer.ToggleDrawer();
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //κλέινουμε το menu
            navigationDrawer.ToggleDrawer();

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
            headerLabel.Text = e.SelectedItem.ToString();
        }
    }
}
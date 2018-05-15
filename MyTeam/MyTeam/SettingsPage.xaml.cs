using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTeam.Models;
using MyTeam.Models.Styles;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DataRow = System.Data.DataRow;

namespace MyTeam
{
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        //Todo: Να έρχονται οι τιμές από τα αποθηκευμένα settings
        public static int RssFeedItems;
        public static string TeamChosen;
        public static string TeamLabel;

        public SettingsPage()
        {
            InitializeComponent();

            FillPickerWithTeams();

            FillAvailableSitesDataGrid(TeamChosen);

            //Ορίζουμε το θέμα για το datagrid
            AvailableSitesDataGrid.GridStyle = new CustomGridStyle();
        }

        //Παίρνουμε όλες τις διαθέσιμές ομάδες με distinct από τα διαθέσιμα RSS που έχει ο κεντρικός πίνακας πληροφοριών
        private void FillPickerWithTeams()
        {
            DataView dv = new DataView(App.TeamsInfoDataTable);
            DataTable distinctTeamsDataTable = dv.ToTable(true, "teamName", "teamLabel");

            foreach (DataRow row in distinctTeamsDataTable.Rows)
            {
                Picker.Items.Add(row["teamLabel"].ToString());
            }

        }

        private void FillAvailableSitesDataGrid(string teamSelected)
        {
            List<AvailableSitesModel> results = new List<AvailableSitesModel>();

            //Σε περίπτωση αρχικοποίσης η τιμή teamSelected δεν θα έχει τιμή οπότε δημιουργούμε μια κενή "dummy" γραμμή
            //TODO: Απενεργοποίηση του κουμπιού αποθήκευσης ρυθμίσεων μέχρι να γίνει επιλογή ομάδας
            if (teamSelected == String.Empty)
            {
                results.Add(new AvailableSitesModel
                {

                    SiteName = "No Data",
                    //TODO: Θέλουμε αν υπήρχε στις προηγούμενες επιλογές του χρήστη να κρατάει τη ρύθμιση
                    SiteSelected = false
                });

            }



            //Παίρνουμε όλα τα διαθέσιμα site για την ομάδα που διάλεξε ο χρήστης
            DataView dv = new DataView(App.TeamsInfoDataTable) { RowFilter = "TeamName = '" + TeamChosen + "'" };
            DataTable availableSitesDataTable = dv.ToTable(true, "SiteName");



            foreach (DataRow row in availableSitesDataTable.Rows)
            {
                results.Add(new AvailableSitesModel
                {
                    SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + row["siteName"] + ".png"),
                    SiteName = row["siteName"].ToString(),
                    //TODO: Θέλουμε αν υπήρχε στις προηγούμενες επιλογές του χρήστη να κρατάει τη ρύθμιση
                    SiteSelected = true
                });

            }
            //Κάνουμε bind τα αποτελέσματα στο dataGrid
            AvailableSitesDataGrid.ItemsSource = new ObservableCollection<AvailableSitesModel>(results);
        }

        private void Picker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //Από το string της επιλεγμένης ομάδας παίρνουμε το όνομα της μεταβλητλης
            DataView dv = new DataView(App.TeamsInfoDataTable) { RowFilter = "teamLabel = '" + Picker.Items[Picker.SelectedIndex] + "'" };
            TeamChosen = dv[0]["teamName"].ToString();
            TeamLabel = dv[0]["teamLabel"].ToString();


            FillAvailableSitesDataGrid(TeamChosen);
        }

        private void SaveSettingsButton_OnPressed(object sender, EventArgs e)
        {
            //TODO: Αποθήκευση ρυθμίσεων και μετάβαση στην σελίδα των feed

            DisplayAlert("Αποθήκευση", "Οι ρυθμίσεις αποθηκεύτηκαν", "'νταξει λέμε");
        }
    }
}
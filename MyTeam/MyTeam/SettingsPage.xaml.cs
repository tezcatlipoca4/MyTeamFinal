using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using MyTeam.Models;
using MyTeam.Models.Styles;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Syncfusion.Data;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DataRow = System.Data.DataRow;

namespace MyTeam
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public static int RssFeedItems;

        //Χρησιμοποιείται για να έχουμε την ομάδα που διάλεξε, μέχρι να πατήσει ο χρήστης αποθήκευση και τα site
        private string _teamChosen;


        public SettingsPage()
        {
            InitializeComponent();

            //Αν είναι η πρώτη φορά που ανοιγει η εφαρμογή βγαίνει ενημερωτικό μήνυμα.
            if (TeamChosen == string.Empty)
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Καλώς ήρθατε",
                        "Ευχαριστούμε που κατεβάσατε την εφαρμογή \"Όλα για την ομάδα μου.\" Για να συνεχίσετε παρακαλώ επιλέξτε την αγαπημένη σας ομάδα και τις ιστοσελίδες από τις οποίες θέλετε να λαμβάνετε ενημερώσεις.",
                        "ΟΚ");
                });


            FillPickerWithTeams();

            FillAvailableSitesDataGrid(TeamChosen);

            Picker.SelectedItem = TeamLabel;

            //Ορίζουμε το θέμα για το datagrid
            AvailableSitesDataGrid.GridStyle = new CustomGridStyle();
        }

        private static ISettings AppSettings => CrossSettings.Current;

        //Παίρνουμε όλες τις διαθέσιμές ομάδες με distinct από τα διαθέσιμα RSS που έχει ο κεντρικός πίνακας πληροφοριών
        private void FillPickerWithTeams()
        {
            DataView dv = new DataView(App.TeamsInfoDataTable);
            DataTable distinctTeamsDataTable = dv.ToTable(true, "teamName", "teamLabel");

            foreach (DataRow row in distinctTeamsDataTable.Rows)
                Picker.Items.Add(row["teamLabel"].ToString());
        }

        private void FillAvailableSitesDataGrid(string teamSelected)
        {
            List<AvailableSitesModel> results = new List<AvailableSitesModel>();

            //Σε περίπτωση αρχικοποίσης η τιμή teamSelected δεν θα έχει τιμή οπότε δημιουργούμε μια κενή "dummy" γραμμή
            //TODO: Απενεργοποίηση του κουμπιού αποθήκευσης ρυθμίσεων μέχρι να γίνει επιλογή ομάδας
            if (teamSelected == string.Empty)
                results.Add(new AvailableSitesModel
                {
                    SiteName = "No Data",
                    //TODO: Θέλουμε αν υπήρχε στις προηγούμενες επιλογές του χρήστη να κρατάει τη ρύθμιση
                    SiteSelected = false
                });


            //Παίρνουμε όλα τα διαθέσιμα site για την ομάδα που διάλεξε ο χρήστης
            DataView dv = new DataView(App.TeamsInfoDataTable) { RowFilter = "TeamName = '" + _teamChosen + "'" };
            DataTable availableSitesDataTable = dv.ToTable(true, "SiteName");


            foreach (DataRow row in availableSitesDataTable.Rows)
                results.Add(new AvailableSitesModel
                {
                    SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + row["siteName"] + ".png"),
                    SiteName = row["siteName"].ToString(),
                    //TODO: Θέλουμε αν υπήρχε στις προηγούμενες επιλογές του χρήστη να κρατάει τη ρύθμιση
                    SiteSelected = true
                });
            //Κάνουμε bind τα αποτελέσματα στο dataGrid
            AvailableSitesDataGrid.ItemsSource = new ObservableCollection<AvailableSitesModel>(results);
        }

        private void Picker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //Από το string της επιλεγμένης ομάδας παίρνουμε το όνομα της μεταβλητλης
            DataView dv = new DataView(App.TeamsInfoDataTable)
            {
                RowFilter = "teamLabel = '" + Picker.Items[Picker.SelectedIndex] + "'"
            };
            _teamChosen = dv[0]["teamName"].ToString();

            FillAvailableSitesDataGrid(_teamChosen);
        }

        private void SaveSettingsButton_OnPressed(object sender, EventArgs e)
        {
            string sitesFilter = GetSelectedSitesFilter();

            //Έλεγχος αν ο χρήστης δεν έχει κανένα site ενεργό
            if (sitesFilter.Equals(string.Empty))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Σφάλμα",
                        "Πρέπει να έχετε επιλέξει τουλάχιστον μια ιστοσελίδα για να ποθηκευτούν οι ρυθμίσεις!", "ΟΚ");
                });

                return;
            }

            //Έλεγχος αν ο χρήστης δεν έχει πραγματοποιήσει αλλαγές σε σχέση με πριν
            if (!sitesFilter.Equals(SitesFilter) || !_teamChosen.Equals(TeamChosen))
            {
                TeamChosen = _teamChosen;
                TeamLabel = Picker.SelectedItem.ToString();
                SitesFilter = sitesFilter;
                FcTableTeamChange();
                App.FilteredByTeamAndSiteDataTable = App.FilterResutlsDataTable();
                App.CurrentLoadedRssModels.Clear();
            }
            Application.Current.MainPage = new MainPage();
        }

        private string GetSelectedSitesFilter()
        {
            string filter = string.Empty;

            foreach (RecordEntry entry in AvailableSitesDataGrid.View.Records)
                if ((bool)AvailableSitesDataGrid.GetCellValue(entry.Data, "SiteSelected"))
                    filter += "'" + AvailableSitesDataGrid.GetCellValue(entry.Data, "SiteName") + "',";

            //Αν ο χρήστης δεν διάλεξε τπτ επιστρέφουμε empty αλλιώς αφαιρούμε το τελευταίο ',' που δημιουργήθηκε από την foreach
            return filter.Equals(string.Empty) ? string.Empty : filter.Substring(0, filter.Length - 1);
        }

        //Βάσει της επιλεγμένης ομάδας αλλάζουμε τα string που χρησιμοποιούνται για τα site της fcTables
        private void FcTableTeamChange()
        {
            switch (TeamChosen)
            {
                case "paok":
                    TeamChosenFcTables = "paok-thessaloniki-fc-191542";
                    break;

                case "aek":
                    TeamChosenFcTables = "aek-athens-179288";
                    break;

                case "osfp":
                    TeamChosenFcTables = "olympiacos-191194";
                    break;

                case "panathinaikos":
                    TeamChosenFcTables = "panathinaikos-191512";
                    break;
            }

            TeamChosenFcTablesNumberOnly = new string(TeamChosenFcTables.Where(char.IsDigit).ToArray());
        }

        #region SettingsVariables

        public static string TeamChosen
        {
            get => AppSettings.GetValueOrDefault(nameof(TeamChosen), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(TeamChosen), value);
        }

        public static string TeamLabel
        {
            get => AppSettings.GetValueOrDefault(nameof(TeamLabel), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(TeamLabel), value);
        }

        public static string SitesFilter
        {
            get => AppSettings.GetValueOrDefault(nameof(SitesFilter), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(SitesFilter), value);
        }

        public static string TeamChosenFcTables
        {
            get => AppSettings.GetValueOrDefault(nameof(TeamChosenFcTables), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(TeamChosenFcTables), value);
        }

        public static string TeamChosenFcTablesNumberOnly
        {
            get => AppSettings.GetValueOrDefault(nameof(TeamChosenFcTablesNumberOnly), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(TeamChosenFcTablesNumberOnly), value);
        }

        #endregion
    }
}
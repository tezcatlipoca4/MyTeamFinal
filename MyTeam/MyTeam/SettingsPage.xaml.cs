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
        private int _numberOfRssFeedItems;
        
        //Χρησιμοποιείται για να έχουμε την ομάδα που διάλεξε, μέχρι να πατήσει ο χρήστης αποθήκευση και τα site και το πλήθος των rss ανά σελίδα
        private string _teamChosen;
        
        public SettingsPage()
        {
            InitializeComponent();

            // Αν είναι η πρώτη φορά που ανοιγει η εφαρμογή βγαίνει ενημερωτικό μήνυμα.
            if (App.TutorialMode)
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Καλώς ήρθατε",
                        "Ευχαριστούμε που κατεβάσατε την εφαρμογή \n\"Όλα για την ομάδα μου!\"\nΓια να συνεχίσετε παρακαλώ επιλέξτε την αγαπημένη σας ομάδα στο επάνω μέρος της οθόνης !",
                        "ΟΚ");
                });

            FillPickerWithTeams();

            //Γεμίζουμε τον picker με το πλήθος άρθρων που θα εμφανίζονται ανά σελίδα
            articlePicker.Items.Add(5.ToString());
            articlePicker.Items.Add(10.ToString());
            articlePicker.Items.Add(15.ToString());

            if (TeamChosen != string.Empty)
            {
                FillAvailableSitesDataGrid(TeamChosen);
                Picker.SelectedItem = TeamLabel;
            }
            articlePicker.SelectedItem = NumberOfRssFeedItems.ToString();

            //Μαρκάρουμε τις επιλογές που έχει ήδη επιλέξει ο χρήστης από το string SitesSelected

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

            //Παίρνουμε όλα τα διαθέσιμα site για την ομάδα που διάλεξε ο χρήστης
            DataView dv = new DataView(App.TeamsInfoDataTable) {RowFilter = "TeamName = '" + _teamChosen + "'"};
            DataTable availableSitesDataTable = dv.ToTable(true, "SiteName");

            foreach (DataRow row in availableSitesDataTable.Rows)

                results.Add(new AvailableSitesModel
                {
                    SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + row["siteName"] + ".png"),
                    SiteName = row["siteName"].ToString(),
                    //TODO: Θέλουμε αν υπήρχε στις προηγούμενες επιλογές του χρήστη να κρατάει τη ρύθμιση
                    SiteSelected = SitesSelectedString.Contains(row["siteName"].ToString())
                });

            //Κάνουμε bind τα αποτελέσματα στο dataGrid

            AvailableSitesDataGrid.ItemsSource = new ObservableCollection<AvailableSitesModel>(results);
        }

        private void Picker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (Picker.Items.Count == 0) return;

            //Από το string της επιλεγμένης ομάδας παίρνουμε το όνομα της μεταβλητλης
            DataView dv = new DataView(App.TeamsInfoDataTable)
            {
                RowFilter = "teamLabel = '" + Picker.Items[Picker.SelectedIndex] + "'"
            };
            _teamChosen = dv[0]["teamName"].ToString();

            FillAvailableSitesDataGrid(_teamChosen);

            if (App.TutorialMode && FirstTimeTeamSelection)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Όλα για την Ομάδα μου",
                        "Τώρα, επιλέξτε από ποιες ιστοσελίδες θέλετε να λαμβάνεται νέα για την ομάδα σας και πατήστε το ΑΠΟΘΗΚΕΥΣΗ.",
                        "ΚΑΤΑΛΑΒΑ");
                });
                FirstTimeTeamSelection = false;
            }
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
                        "Πρέπει να έχετε επιλέξει τουλάχιστον μια ιστοσελίδα για να αποθηκευτούν οι ρυθμίσεις!", "ΟΚ");
                });

                return;
            }

            //Έλεγχος αν ο χρήστης δεν έχει πραγματοποιήσει αλλαγές σε σχέση με πριν
            if (!sitesFilter.Equals(SitesFilter) || !_teamChosen.Equals(TeamChosen) ||
                _numberOfRssFeedItems != NumberOfRssFeedItems)
            {
                TeamChosen = _teamChosen;
                TeamLabel = Picker.SelectedItem.ToString();
                SitesFilter = sitesFilter;
                FcTableTeamChange();
                NumberOfRssFeedItems = _numberOfRssFeedItems;
                App.FilteredByTeamAndSiteDataTable = App.FilterResutlsDataTable();
                App.CurrentLoadedRssModels.Clear();

                //Φορτώνουμε το string που περιέχει όλες τις ιστοσελίδες που έχει ο χρήστης
                SitesSelectedString = CreateSitesSelectedString();
            }
            Application.Current.MainPage = new MainPage();
        }

        private string CreateSitesSelectedString()
        {
            string tempString = string.Empty;
            foreach (var item in AvailableSitesDataGrid.View.Records)
            {
                if (!(bool) AvailableSitesDataGrid.GetCellValue(item.Data, "SiteSelected")) continue;

                tempString += "," + AvailableSitesDataGrid.GetCellValue(item.Data, "SiteName");
            }

            //Σβήνουνμε το πρώτο ','
            return tempString.Remove(0, 1);
        }

        private string GetSelectedSitesFilter()
        {
            string filter = string.Empty;

            foreach (RecordEntry entry in AvailableSitesDataGrid.View.Records)
                if ((bool) AvailableSitesDataGrid.GetCellValue(entry.Data, "SiteSelected"))
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

        private void articlePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (articlePicker.Items.Count == 0) return;

            _numberOfRssFeedItems = Convert.ToInt32(articlePicker.SelectedItem);
        }

        #region SettingsVariables

        private static bool FirstTimeTeamSelection
        {
            get => AppSettings.GetValueOrDefault(nameof(FirstTimeTeamSelection), true);
            set => AppSettings.AddOrUpdateValue(nameof(FirstTimeTeamSelection), value);
        }

        public static int NumberOfRssFeedItems
        {
            get => AppSettings.GetValueOrDefault(nameof(NumberOfRssFeedItems), 15);
            set => AppSettings.AddOrUpdateValue(nameof(NumberOfRssFeedItems), value);
        }

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

        public static string SitesSelectedString
        {
            get => AppSettings.GetValueOrDefault(nameof(SitesSelectedString), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(SitesSelectedString), value);
        }

        #endregion
    }
}
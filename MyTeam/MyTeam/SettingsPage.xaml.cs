﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using MyTeam.Models;
using MyTeam.Models.Styles;
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
        //Todo: Να έρχονται οι τιμές από τα αποθηκευμένα settings
        public static int RssFeedItems;

        public static string TeamChosen;
        public static string TeamLabel;
        public static string SitesFilter;

        //Χρησιμοποιείται για να έχουμε την ομάδα που διάλεξε, μέχρι να πατήσει ο χρήστης αποθήκευση και τα site
        private string _teamChosen;


        public SettingsPage()
        {
            InitializeComponent();

            FillPickerWithTeams();

            FillAvailableSitesDataGrid(TeamChosen);

            Picker.SelectedItem = TeamLabel;
            //Ορίζουμε το θέμα για το datagrid
            AvailableSitesDataGrid.GridStyle = new CustomGridStyle();
        }

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
                //DisplayAlert("Σφάλμα",
                //    "Πρέπει να έχετε επιλέξει τουλάχιστον μια ιστοσελίδα για να ποθηκευτούν οι ρυθμίσεις!", "ΟΚ");

                //Device.BeginInvokeOnMainThread(async () =>
                //{
                //    await DisplayAlert("Σφάλμα",
                //        "Πρέπει να έχετε επιλέξει τουλάχιστον μια ιστοσελίδα για να ποθηκευτούν οι ρυθμίσεις!", "Ok");
                //});
                return;
            }

            //Έλεγχος αν ο χρήστης δεν έχει πραγματοποιήσει αλλαγές σε σχέση με πριν
            if (sitesFilter.Equals(SitesFilter) && _teamChosen.Equals(TeamChosen)) return;

            //TODO: Αποθήκευση ρυθμίσεων και μετάβαση στην σελίδα των feed
            TeamChosen = _teamChosen;
            TeamLabel = Picker.SelectedItem.ToString();
            SitesFilter = sitesFilter;
            App.FilteredByTeamAndSiteDataTable = App.FilterResutlsDataTable();
            App.CurrentLoadedRssModels.Clear();
            
        }

        private string GetSelectedSitesFilter()
        {
            string filter = string.Empty;

            foreach (RecordEntry entry in AvailableSitesDataGrid.View.Records)
            {
                if ((bool)AvailableSitesDataGrid.GetCellValue(entry.Data, "SiteSelected"))
                {
                    filter += "'" + AvailableSitesDataGrid.GetCellValue(entry.Data, "SiteName") + "',";
                }
            }

            //Αν ο χρήστης δεν διάλεξε τπτ επιστρέφουμε empty αλλιώς αφαιρούμε το τελευταίο ',' που δημιουργήθηκε από την foreach
            return filter.Equals(string.Empty) ? string.Empty : filter.Substring(0, filter.Length - 1);
        }
    }
}
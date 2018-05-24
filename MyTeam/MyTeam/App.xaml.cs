using System;
using System.Collections.Generic;
using System.Data;
using MyTeam.Models;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace MyTeam
{
    public partial class App : Application
    {
        //Ο πίνακας που περιέχει όλες τις πληροφορίες για όλες τις ομάδες και όλα τα site
        public static DataTable TeamsInfoDataTable = new DataTable();

        public static List<RssModel> CurrentLoadedRssModels = new List<RssModel>();
        public static DateTime LastLoadedDateTime;

        //TODO: Αυτός ο πίνακας θα ενημερώνεται στο άνοιγμα της εφαρμογής βάσει των αποθηκευμένων επιλογών και με την αποθήκευση των ρυθμίσεων από τον χρήστη
        //Περιέχει τα φιλτραρισμένα στοιχεία και url που θα χρησιμοποιούμε για να τραβηξουμε τα RSS feeds
        public static DataTable FilteredByTeamAndSiteDataTable = new DataTable();


        public App()
        {
            InitializeComponent();

            //Γέμισμα πίνακα με όλες τις πληροφορίες
            FillTeamInfoDataTable(TeamsInfoDataTable);

            //Todo: αφαίρεση της δήλωσης της ομάδας απο εδώ, θα έρχεται από τα settings
            //SettingsPage.TeamChosen = "paok";
            //SettingsPage.TeamLabel = "ΠΑΟΚ";
            //SettingsPage.SitesFilter = "'Contra','Gazzetta','NovaSports','OnSports','Sport24'";

            FilteredByTeamAndSiteDataTable = FilterResutlsDataTable();

            //MainPage = new AboutPage();
            //Ελέγχουμε αν είναι η πρώτη εκτέλεση της εφαρμογής ώστε να στείλουμε τον χρήστη στις ρυθμίσεις
            MainPage = SettingsPage.TeamChosen == string.Empty ? (Page) new AboutPage() : new MainPage();
        }

        //Η μέθοδος φιλτράρει τον πίνακα με όλες τις πληροφορίες ώστε να πάρουμε μόνο τα feed για την ομάδα και τα site που έχουμε επιλέξει
        public static DataTable FilterResutlsDataTable()
        {
            DataView dv = TeamsInfoDataTable.DefaultView;

            if (SettingsPage.TeamChosen != string.Empty)
                dv.RowFilter = "teamName = '" + SettingsPage.TeamChosen + "' AND siteName IN (" +
                               SettingsPage.SitesFilter + ")";

            return dv.ToTable();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        // Software Keys Interface
        public interface IHasHardwareKeys
        {            
            bool IsNavigationBarAvailable();
        }

        public static bool IsDeviceConnected()
        {
            return CrossConnectivity.Current.IsConnected;
        }

        //Δημιουργία πίνακα με τις πληροφορίες και τα url των ιστοσελίδων
        private void FillTeamInfoDataTable(DataTable table)
        {
            table.Columns.Add("teamName", typeof(string));
            table.Columns.Add("teamLabel", typeof(string));
            table.Columns.Add("siteName", typeof(string));
            table.Columns.Add("rssType", typeof(string));
            table.Columns.Add("url", typeof(string));

            //ΑΕΚ
            table.Rows.Add("aek", "ΑΕΚ", "AEK365", "RSS", "http://feeds.feedburner.com/aek365gr-aek");
            table.Rows.Add("aek", "ΑΕΚ", "Contra", "Atom", "http://www.contra.gr/soccer/aek/?widget=rssfeed&view=feed");
            table.Rows.Add("aek", "ΑΕΚ", "Gazzetta", "RSS", "http://www.gazzetta.gr/taxonomy/term/1001/all/feed");
            table.Rows.Add("aek", "ΑΕΚ", "NovaSports", "RSS",
                "http://www.novasports.gr/sys/novasports/RssFeed/GetFeed?type=2&id=16673&languageID=1");
            table.Rows.Add("aek", "ΑΕΚ", "OnSports", "RSS", "http://www.onsports.gr/omades/aek?format=feed&type=rss");
            table.Rows.Add("aek", "ΑΕΚ", "Sport24", "Atom",
                "http://www.sport24.gr/football/omades/aek/?widget=rssfeed&view=feed&contentId=174866");
            //osfp
            table.Rows.Add("osfp", "ΟΣΦΠ", "Contra", "Atom",
                "http://www.contra.gr/soccer/olympiacos/?widget=rssfeed&view=feed");
            table.Rows.Add("osfp", "ΟΣΦΠ", "Gayros", "RSS", "http://gavros.gr/rss?projection=370");
            table.Rows.Add("osfp", "ΟΣΦΠ", "Gazzetta", "RSS", "http://www.gazzetta.gr/taxonomy/term/1014/all/feed");
            table.Rows.Add("osfp", "ΟΣΦΠ", "NovaSports", "RSS",
                "http://www.novasports.gr/sys/novasports/RssFeed/GetFeed?type=2&id=16641&languageID=1");
            table.Rows.Add("osfp", "ΟΣΦΠ", "OnSports", "RSS",
                "http://www.onsports.gr/omades/olympiakos?format=feed&type=rss");
            table.Rows.Add("osfp", "ΟΣΦΠ", "Sport24", "Atom",
                "http://www.sport24.gr/football/omades/olympiakos/?widget=rssfeed&view=feed&contentId=174866");
            //pao
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "Contra", "Atom",
                "http://www.contra.gr/soccer/panathinaikos/?widget=rssfeed&view=feed");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "Gazzetta", "RSS",
                "http://www.gazzetta.gr/taxonomy/term/1016/all/feed");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "NovaSports", "RSS",
                "http://www.novasports.gr/sys/novasports/RssFeed/GetFeed?type=2&id=16625&languageID=1");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "OnSports", "RSS",
                "http://www.onsports.gr/omades/panathinaikos?format=feed&type=rss");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "PrasinaNea", "RSS", "http://www.prasinanea.gr/feed/");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "Sport24", "Atom",
                "http://www.sport24.gr/football/omades/paok/?widget=rssfeed&view=feed&contentId=174866");
            
            //paok
            table.Rows.Add("paok", "ΠΑΟΚ", "Contra", "Atom",
                "http://www.contra.gr/soccer/paok/?widget=rssfeed&view=feed&contentId=1169269");
            table.Rows.Add("paok", "ΠΑΟΚ", "Gazzetta", "RSS", "http://www.gazzetta.gr/taxonomy/term/1018/all/feed");
            //table.Rows.Add("paok", "ΠΑΟΚ", "inPAOK", "RSS", "http://inpaok.com/feed/");
            table.Rows.Add("paok", "ΠΑΟΚ", "NovaSports", "RSS",
                "http://www.novasports.gr/sys/novasports/RssFeed/GetFeed?type=2&id=16657&languageID=1");
            table.Rows.Add("paok", "ΠΑΟΚ", "OnSports", "RSS",
                "http://www.onsports.gr/omades/paok?format=feed&type=rss");
            table.Rows.Add("paok", "ΠΑΟΚ", "Sport24", "Atom",
                "http://www.sport24.gr/football/omades/paok/?widget=rssfeed&view=feed&contentId=174866");
        }

        
    }
}
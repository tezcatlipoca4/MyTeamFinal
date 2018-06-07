using System;
using System.Collections.Generic;
using System.Data;
using MyTeam.Models;
using Plugin.Connectivity;
using Xamarin.Forms;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MyTeam
{
    public partial class App : Application
    {
        //Ο πίνακας που περιέχει όλες τις πληροφορίες για όλες τις ομάδες και όλα τα site
        public static DataTable TeamsInfoDataTable = new DataTable();

        public static List<RssModel> CurrentLoadedRssModels = new List<RssModel>();
        public static List<RssModel> CurrentGeneralLoadedRssModels = new List<RssModel>();

        public static DateTime LastLoadedDateTime;
        public static DateTime LastGeneralLoadedDateTime;

        private static ISettings AppSettings => CrossSettings.Current;
		public static bool TutorialMode 
		{
			get => AppSettings.GetValueOrDefault(nameof(TutorialMode), true);
			set => AppSettings.AddOrUpdateValue(nameof(TutorialMode), value);
		}


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
            // MainPage = SettingsPage.TeamChosen == string.Empty ? (Page) new AboutPage() : new MainPage();
			MainPage = TutorialMode ? (Page)new AboutPage() : new MainPage();
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

        // Get version Interface
		public interface IGetVersionNumber
		{
			string GetVersion();
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

            table.Rows.Add("aek", "ΑΕΚ", "AEK365", "RSS", "http://feeds.feedburner.com/aek365gr-aek");
            table.Rows.Add("aek", "ΑΕΚ", "Contra", "Atom", "http://www.contra.gr/soccer/aek/?widget=rssfeed&view=feed");
            table.Rows.Add("aek", "ΑΕΚ", "Gazzetta", "RSS", "http://www.gazzetta.gr/taxonomy/term/1001/all/feed");
            table.Rows.Add("aek", "ΑΕΚ", "NovaSports", "RSS", "http://www.novasports.gr/sys/novasports/RssFeed/GetFeed?type=2&id=16673&languageID=1");
            table.Rows.Add("aek", "ΑΕΚ", "OnSports", "Html", "https://www.onsports.gr/omades/aek");
            table.Rows.Add("aek", "ΑΕΚ", "SDNA", "Html", "http://www.sdna.gr/teams/aek");
            table.Rows.Add("aek", "ΑΕΚ", "Sport24", "Atom", "http://www.sport24.gr/football/omades/aek/?widget=rssfeed&view=feed&contentId=174866");
            table.Rows.Add("aek", "ΑΕΚ", "SportFM", "Html", "http://www.sport-fm.gr/tag/aek");
            table.Rows.Add("aris", "Άρης", "ArisFC", "RSS", "http://arisfc.com.gr/index.php/enimerosi/category-items/4-news/2-last-news?format=feed&type=rss");
            table.Rows.Add("aris", "Άρης", "Contra", "Atom", "http://www.contra.gr/soccer/aris/?widget=rssfeed&view=feed");
            table.Rows.Add("aris", "Άρης", "Gazzetta", "RSS", "http://www.gazzetta.gr/taxonomy/term/1006/all/feed");
            table.Rows.Add("aris", "Άρης", "NovaSports", "RSS", "http://www.novasports.gr/sys/novasports/RssFeed/GetFeed?type=2&id=16817&languageID=1");
            table.Rows.Add("aris", "Άρης", "OnSports", "Html", "https://www.onsports.gr/omades/aris");
            table.Rows.Add("aris", "Άρης", "Planetaris", "RSS", "http://feeds.feedburner.com/planetaris");
            table.Rows.Add("aris", "Άρης", "SDNA", "Html", "http://www.sdna.gr/teams/aris");
            table.Rows.Add("aris", "Άρης", "Sport24", "Atom", "http://www.sport24.gr/football/omades/aris/?widget=rssfeed&view=feed&contentId=174866");
            table.Rows.Add("aris", "Άρης", "SportFM", "Html", "http://www.sport-fm.gr/tag/aris");
            table.Rows.Add("atromitos", "Ατρόμητος", "AtromitosFC", "RSS", "http://www.atromitosfc.gr/el/news-el/news-el.feed?type=rss");
            table.Rows.Add("atromitos", "Ατρόμητος", "Contra", "Atom", "http://www.contra.gr/soccer/atromitos/?widget=rssfeed&view=feed");
            table.Rows.Add("atromitos", "Ατρόμητος", "Gazzetta", "RSS", "http://www.gazzetta.gr/taxonomy/term/1008/all/feed");
            table.Rows.Add("atromitos", "Ατρόμητος", "NovaSports", "RSS", "http://www.novasports.gr/sys/novasports/RssFeed/GetFeed?type=2&id=17073&languageID=1");
            table.Rows.Add("atromitos", "Ατρόμητος", "OnSports", "Html", "https://www.onsports.gr/omades/atromitos");
            table.Rows.Add("atromitos", "Ατρόμητος", "SDNA", "Html", "http://www.sdna.gr/teams/atromitos");
            table.Rows.Add("atromitos", "Ατρόμητος", "Sport24", "Atom", "http://www.sport24.gr/football/omades/atromitos/?widget=rssfeed&view=feed&contentId=174866");
            table.Rows.Add("atromitos", "Ατρόμητος", "SportFM", "Html", "http://www.sport-fm.gr/tag/atromitos");
            table.Rows.Add("general", "Γενικές Ειδήσεις", "Contra", "Atom", "http://www.contra.gr/latest/?widget=rssfeed&view=feed&contentId=1169269");
            table.Rows.Add("general", "Γενικές Ειδήσεις", "Gazzetta", "RSS", "http://www.gazzetta.gr/rssfeeds/allnewsfeed");
            table.Rows.Add("general", "Γενικές Ειδήσεις", "NovaSports", "RSS", "http://www.novasports.gr/sys/novasports/RssFeed/GetFeed?type=999&id=1&languageID=1");
            table.Rows.Add("general", "Γενικές Ειδήσεις", "OnSports", "RSS", "https://www.onsports.gr/latest-news?format=feed");
            table.Rows.Add("general", "Γενικές Ειδήσεις", "SDNA", "RSS", "http://www.sdna.gr/latest.xml");
            table.Rows.Add("general", "Γενικές Ειδήσεις", "Sport24", "Atom", "http://www.sport24.gr/latest/?widget=rssfeed&view=feed&contentId=174866");
            table.Rows.Add("general", "Γενικές Ειδήσεις", "SportFM", "Html", "http://www.sport-fm.gr/archive/latest/");
            table.Rows.Add("osfp", "Ολυμπιακός", "Contra", "Atom", "http://www.contra.gr/soccer/olympiacos/?widget=rssfeed&view=feed");
            table.Rows.Add("osfp", "Ολυμπιακός", "Gazzetta", "RSS", "http://www.gazzetta.gr/taxonomy/term/1014/all/feed");
            table.Rows.Add("osfp", "Ολυμπιακός", "NovaSports", "RSS", "http://www.novasports.gr/sys/novasports/RssFeed/GetFeed?type=2&id=16641&languageID=1");
            table.Rows.Add("osfp", "Ολυμπιακός", "OlympiacosFC", "RSS", "http://www.olympiacos.org/feeds/social");
            table.Rows.Add("osfp", "Ολυμπιακός", "OnSports", "Html", "https://www.onsports.gr/omades/olympiakos");
            table.Rows.Add("osfp", "Ολυμπιακός", "SDNA", "Html", "http://www.sdna.gr/teams/olympiakos");
            table.Rows.Add("osfp", "Ολυμπιακός", "Sport24", "Atom", "http://www.sport24.gr/football/omades/olympiakos/?widget=rssfeed&view=feed&contentId=174866");
            table.Rows.Add("osfp", "Ολυμπιακός", "SportFM", "Html", "http://www.sport-fm.gr/tag/olympiakos");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "Contra", "Atom", "http://www.contra.gr/soccer/panathinaikos/?widget=rssfeed&view=feed");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "Gazzetta", "RSS", "http://www.gazzetta.gr/taxonomy/term/1016/all/feed");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "NewsPAO", "RSS", "http://www.newsPAO.gr/feed/");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "NovaSports", "RSS", "http://www.novasports.gr/sys/novasports/RssFeed/GetFeed?type=2&id=16625&languageID=1");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "OnSports", "Html", "https://www.onsports.gr/omades/panathinaikos");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "SDNA", "Html", "http://www.sdna.gr/teams/panathinaikos");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "Sport24", "Atom", "http://www.sport24.gr/football/omades/panathinaikos/?widget=rssfeed&view=feed&contentId=174866");
            table.Rows.Add("panathinaikos", "Παναθηναϊκός", "SportFM", "Html", "http://www.sport-fm.gr/tag/pao");
            table.Rows.Add("paok", "ΠΑΟΚ", "Contra", "Atom", "http://www.contra.gr/soccer/paok/?widget=rssfeed&view=feed&contentId=1169269");
            table.Rows.Add("paok", "ΠΑΟΚ", "Gazzetta", "RSS", "http://www.gazzetta.gr/taxonomy/term/1018/all/feed");
            table.Rows.Add("paok", "ΠΑΟΚ", "NovaSports", "RSS", "http://www.novasports.gr/sys/novasports/RssFeed/GetFeed?type=2&id=16657&languageID=1");
            table.Rows.Add("paok", "ΠΑΟΚ", "OnSports", "Html", "https://www.onsports.gr/omades/paok");
            table.Rows.Add("paok", "ΠΑΟΚ", "PAOK24", "Html", "http://www.paok24.com/roh/paok");
            table.Rows.Add("paok", "ΠΑΟΚ", "PAOKFC", "RSS", "http://www.paokfc.gr/category/nea/feed/");
            table.Rows.Add("paok", "ΠΑΟΚ", "SDNA", "Html", "http://www.sdna.gr/teams/paok");
            table.Rows.Add("paok", "ΠΑΟΚ", "Sport24", "Atom", "http://www.sport24.gr/football/omades/paok/?widget=rssfeed&view=feed&contentId=174866");
            table.Rows.Add("paok", "ΠΑΟΚ", "SportFM", "Html", "http://www.sport-fm.gr/tag/paok");


        }


    }
}
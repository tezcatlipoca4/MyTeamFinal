using System.Data;
using Xamarin.Forms;

namespace MyTeam
{
    public partial class App : Application
    {
        //Ο πίνακας που περιέχει όλες τις πληροφορίες για όλες τις ομάδες και όλα τα site
        public static DataTable TeamsInfoDataTable = new DataTable();

        //TODO: Αυτός ο πίνακας θα ενημερώνεται στο άνοιγμα της εφαρμογής βάσει των αποθηκευμένων επιλογών και με την αποθήκευση των ρυθμίσεων από τον χρήστη
        //Περιέχει τα φιλτραρισμένα στοιχεία και url που θα χρησιμοποιούμε για να τραβηξουμε τα RSS feeds
        public static DataTable FilteredByTeamAndSiteDataTable = new DataTable();

        public static string TeamChosen;

        public App()
        {
            InitializeComponent();
            //Todo: αφαίρεση της δήλωσης της ομάδας απο εδώ, θα έρχεται από τα settings
            TeamChosen = "paok";


            //Δημιουργία πίνακα με τις πληροφορίες και τα url των ιστοσελίδων

            TeamsInfoDataTable.Columns.Add("teamName", typeof(string));
            TeamsInfoDataTable.Columns.Add("teamLabel", typeof(string));
            TeamsInfoDataTable.Columns.Add("siteName", typeof(string));
            TeamsInfoDataTable.Columns.Add("rssType", typeof(string));
            TeamsInfoDataTable.Columns.Add("url", typeof(string));

            //ΑΕΚ
            TeamsInfoDataTable.Rows.Add("aek", "ΑΕΚ","gazzetta", "RSS", "http://www.gazzetta.gr/taxonomy/term/1001/all/feed");
            TeamsInfoDataTable.Rows.Add("aek", "ΑΕΚ", "sport24", "Atom", "http://www.sport24.gr/football/omades/aek/?widget=rssfeed&view=feed&contentId=174866");
            TeamsInfoDataTable.Rows.Add("aek", "ΑΕΚ", "contra", "Atom", "http://www.contra.gr/soccer/aek/?widget=rssfeed&view=feed");


            //ΠΑΟΚ
            TeamsInfoDataTable.Rows.Add("paok", "ΠΑΟΚ", "gazzetta", "RSS",
                "http://www.gazzetta.gr/taxonomy/term/1018/all/feed");
            TeamsInfoDataTable.Rows.Add("paok", "ΠΑΟΚ", "sport24", "Atom",
                "http://www.sport24.gr/football/omades/paok/?widget=rssfeed&view=feed&contentId=174866");
            TeamsInfoDataTable.Rows.Add("paok", "ΠΑΟΚ", "contra", "Atom",
                "http://www.contra.gr/soccer/paok/?widget=rssfeed&view=feed");


            FilteredByTeamAndSiteDataTable =  FilterResutlsDataTable();


            MainPage = new RssFeedPage();
        }

        //Η μέθοδος φιλτράρει τον πίνακα με όλες τις πληροφορίες ώστε να πάρουμε μόνο τα feed για την ομάδα και τα site που έχουμε επιλέξει
        public DataTable FilterResutlsDataTable()
        {
            DataView dv = TeamsInfoDataTable.DefaultView;

            dv.RowFilter = "teamName = '" + App.TeamChosen + "'";

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
    }
}
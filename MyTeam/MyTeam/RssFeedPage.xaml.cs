using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MyTeam.Models;
using Plugin.Connectivity;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DataRow = System.Data.DataRow;

namespace MyTeam
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RssFeedPage : ContentPage
    {
        public RssFeedPage()
        {
            InitializeComponent();
                       
            //Βάζουμε τα εικονίδια στα banner από την ομάδα που έχει επιλέξει ο χρήστης
            LeftBannerTeamLogo.Source = RightBannerTeamLogo.Source =
            ImageSource.FromResource("MyTeam.Assets.Images.teamLogos." + SettingsPage.TeamChosen + ".png");
            teamLabel.Text = SettingsPage.TeamLabel;

            dataGrid.GridLoaded += DataGrid_OnGridLoaded;

            pullToRefresh.Refreshing += PullToRefresh_Refreshing;            
            
        }
        private  void PullToRefresh_Refreshing(object sender, EventArgs e)
        {
            pullToRefresh.IsRefreshing = true;
            App.CurrentLoadedRssModels.Clear();
            dataGrid.ItemsSource = null;
            LoadDataToGrid();

            pullToRefresh.IsRefreshing = false;
        }

        private List<RssModel> GetRssModels()
        {
            //Αν υπήρχαν δεδομένα από πρίν τα φρτώνουμε απο εκεί
            if (App.CurrentLoadedRssModels.Count > 0)
                return App.CurrentLoadedRssModels;
            //Ο πίνακας FilteredBy... έχει φιξ τιμές και δεν χρειάζεται να τον παίρνουμε σαν όρισμα, αλλά τον βάζουμε κατευθείαν μέσα στη μέθοδο
            List<RssModel> combinedResults = new List<RssModel>();

            foreach (DataRow row in App.FilteredByTeamAndSiteDataTable.Rows)
            {
                Device.BeginInvokeOnMainThread(() => ActivityStatusLabel.Text = "Παρακαλώ περιμένετε...\nΦόρτωση ειδήσεων από το\n" + row["siteName"]);
                Device.BeginInvokeOnMainThread(()=> loadingActivitySiteImage.Source = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + row["siteName"] + ".png"));

                //Για κάθε ένα site βάζουμε τις τιμές στο combinedResults
                //TODO: Να επιλέγει ο χρήστης από 5-15 άρθρα από κάθε σελίδα
                List<RssModel> tempResults = GetRssFeed(row["rssType"].ToString(), row["url"].ToString(),
                    row["siteName"].ToString());
                combinedResults.AddRange(tempResults);
            }

            //Ενημερώνουμε την στιγμή που τραβήξαμε τα δεδομένα
            App.LastLoadedDateTime = DateTime.Now;
            return combinedResults;
        }

        public async void LoadDataToGrid()
        {
            //Εμφανίζουμε το stacklayout με το status φόρτωσης δεδομένων
            LoadingStatusStackLayout.IsVisible = true;
            
            App.CurrentLoadedRssModels = await Task.Run(() => GetRssModels());
            dataGrid.ItemsSource = new ObservableCollection<RssModel>(App.CurrentLoadedRssModels);
            FooterLabel.Text = "Τελευταία ενημέρωση: " + App.LastLoadedDateTime.ToString("dd/MM/yy - HH:mm");
            FooterLabel.HorizontalTextAlignment = TextAlignment.Center;

            LoadingStatusStackLayout.IsVisible = false;
        }


        //Επιστρέφει τα πιο πρόσφατα feeds βάσει των τιμων που δίνονται
        public List<RssModel> GetRssFeed(string rssType, string url, string siteName, int numberOfItems = 15)
        {
            //todo:Αν το feed δεν ερχεται σωστα να εημερώνεται ο χρήστης και να αφαιρείται 

            //Δημιουργούμε το XDocument το οποίο είναι κοινό ανεξαιρέτως του τύπου δομής RSS/Atom
            var xDocument = XDocument.Load(url);

            switch (rssType)
            {
                case "RSS":
                    return (from feed in xDocument.Descendants("item")
                            select new RssModel
                            {
                                SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                Title = feed.Element("title").Value,
                                Url = feed.Element("link").Value,
                                PublishedDatetime = DateTime.Parse(feed.Element("pubDate").Value)
                            }).Take(numberOfItems).ToList();

                //Μιας και έχουμε μόνο δύο περιπτώσεις αν δεν είναι RSS θα είναι Atom οπότε μποροπυμε να το αφήσουμε στο default
                default:
                    return (from item in xDocument.Root.Elements().Where(i => i.Name.LocalName == "entry")
                            select new RssModel
                            {
                                SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                Url = item.Elements().First(i => i.Name.LocalName == "link").Attribute("href").Value,
                                Title = item.Elements().First(i => i.Name.LocalName == "title").Value,
                                PublishedDatetime =
                                    DateTime.Parse(item.Elements().First(i => i.Name.LocalName == "updated").Value)
                            }).Take(numberOfItems).ToList();
            }
        }

        //Στο κλικ του χρήστη ανοίγουμε το url που επέλεξε στον browser που χρησιμοποιεί
        private void DataGrid_OnGridTapped(object sender, GridTappedEventArgs e)
        {
            Device.OpenUri(new Uri(dataGrid.GetCellValue(e.RowData, "Url").ToString()));
        }

        public bool IsDeviceConnected()
        {
            return CrossConnectivity.Current.IsConnected;
        }

        private void DataGrid_OnGridLoaded(object sender, GridLoadedEventArgs e)
        {
            if (IsDeviceConnected())
			{
				LoadDataToGrid();
			}
                

        }
    }
}
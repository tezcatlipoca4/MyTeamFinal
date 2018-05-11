using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MyTeam.Models;
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

            //Ορίζουμε την εντολή για το refresh
            dataGrid.PullToRefreshCommand = new Command(ExecutePullToRefreshCommand);

            LoadDataToGrid();
        }

        public void LoadDataToGrid()
        {
            //Ο πίνακας FilteredBy... έχει φιξ τιμές και δεν χρειάζεται να τον παίρνουμε σαν όρισμα, αλλά τον βάζουμε κατευθείαν μέσα στη μέθοδο
            List<RssModel> combinedResults = new List<RssModel>();

            foreach (DataRow row in App.FilteredByTeamAndSiteDataTable.Rows)
            {
                //Για κάθε ένα site βάζουμε τις τιμές στο combinedResults
                //TODO: Να επιλέγει ο χρήστης από 5-15 άρθρα από κάθε σελίδα
                List<RssModel> tempResults = GetRssFeed(row["rssType"].ToString(), row["url"].ToString(),
                    row["siteName"].ToString());
                combinedResults.AddRange(tempResults);
            }

            //Κάνουμε bind τα αποτελέσματα στο dataGrid
            dataGrid.ItemsSource = new ObservableCollection<RssModel>(combinedResults);

            //Ενημερώουμε το footerLabel με την τελευταία ώρα ενημέρωσης
            FooterLabel.Text = "Τελευταία ενημέρωση: " + DateTime.Now.ToString("dd-MM-yy - HH:mm");
        }

        //H μέθοδος επιστρέφει τα πιο πρόσφατα feeds βάσει των τιμων που δίνονται
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

        private void DataGrid_OnGridTapped(object sender, GridTappedEventArgs e)
        {
            //Στο κλικ του χρήστη ανοίγουμε το url που επέλεξε στον browser που χρησιμοποιεί
            Device.OpenUri(new Uri(dataGrid.GetCellValue(e.RowData, "Url").ToString()));
        }

        private async void ExecutePullToRefreshCommand()
        {
            dataGrid.IsBusy = true;

            await Task.Delay(new TimeSpan(0, 0, 5));
            LoadDataToGrid();
            dataGrid.IsBusy = false;
        }

    }
}
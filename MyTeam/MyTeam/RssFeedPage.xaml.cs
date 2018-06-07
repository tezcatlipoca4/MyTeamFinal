using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
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
        public static bool GeneralNewsSelected;

        //Ctor
        public RssFeedPage()
        {
            InitializeComponent();

            //Αν έχουμε γενικές ειδήσεις το banner με την ομάδα δεν θα φαίνεται 

            if (GeneralNewsSelected)
            {
                bannerStackLayout.IsVisible = false;
            }
            else
            {
                //Βάζουμε τα εικονίδια στα banner από την ομάδα που έχει επιλέξει ο χρήστης
                LeftBannerTeamLogo.Source = RightBannerTeamLogo.Source =
                    ImageSource.FromResource("MyTeam.Assets.Images.teamLogos." + SettingsPage.TeamChosen + ".png");
                teamLabel.Text = SettingsPage.TeamLabel;
            }

            dataGrid.GridLoaded += DataGrid_OnGridLoaded;
            pullToRefresh.Refreshing += PullToRefresh_Refreshing;
            dataGrid.GroupCaptionTextFormat = "{Key}";
        }

        #region Properties

        // Όλα τα τυχαία μηνύματα
        public string[] RandomMessagesArray =
        {
            "Ανακρίνουμε Ορκς",
            "Αποστηθίζουμε Θωμά Μάτσιο",
            "Αποφεύγουμε ρολά ταμειακής",
            "Βάφουμε τα δοκάρια",
            "Γυαλίζουμε το κύπελλο",
            "Δένουμε τα κορδόνια στις τάπες",
            "Ισιώνουμε τις γραμμές",
            "Ισιώνουμε τις γραμμες",
            "Μελετάμε Νικολακόπουλο",
            "Μεταφράζουμε Γιώργο Μίνο στα Φιλανδικά",
            "Μοιράζουμε χαρτάκια",
            "Μορφώνουμε Μαο - Μαο",
            "Ξεχνάμε το πιστόλι στη ζώνη μας",
            "Παραγγέλνουμε pizza",
            "Παραγγέλνουμε σουβλάκια",
            "Πετάμε χαρταετό",
            "Ποτίζουμε γκαζόν και Αραούχο",
            "Ποτίζουμε το χορτάρι",
            "Προπονούμε δικαστές",
            "Ράβουμε τα δίχτυα",
            "Ρίχνουμε δυο δελτία στοιχήματος",
            "Σιδερώνουμε τα σημαιάκια",
            "Σκουπίζουμε τις κερκίδες",
            "Στήνουμε το τείχος",
            "Συμβουελυόμαστε τον επόπτη",
            "Ταΐζουμε τον Μπίσεσβαρ",
            "ΤΙΝΑΦΤΟΡΕ",
            "Τσεκάρουμε τις ρίγες στις φανέλες",
            "Φουσκώνουμε τις μπάλες",
            "Ψάχνουμε για ball boys",
            "Ψάχνουμε το διαιτητή"
        };

        public Random randomNumber = new Random();

        #endregion

        #region Methods

        private void PullToRefresh_Refreshing(object sender, EventArgs e)
        {
            pullToRefresh.IsRefreshing = true;

            //Ανάλογα με το από που κάνουμε ανανέωση καθαρίζουμε το ανάλγο RssModel
            if (GeneralNewsSelected)
                App.CurrentGeneralLoadedRssModels.Clear();
            else
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
                Device.BeginInvokeOnMainThread(() =>
                    ActivityStatusLabel.Text = "Παρακαλώ περιμένετε...\nΦόρτωση ειδήσεων από το\n" + row["siteName"]);
                Device.BeginInvokeOnMainThread(() =>
                    loadingActivitySiteImage.Source =
                        ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + row["siteName"] + ".png"));
                Device.BeginInvokeOnMainThread(GenerateRandomLoadingText);


                //Για κάθε ένα site βάζουμε τις τιμές στο combinedResults
                List<RssModel> tempResults = GetRssFeed(row["rssType"].ToString(), row["url"].ToString(),
                        row["siteName"].ToString(), SettingsPage.NumberOfRssFeedItems);
                combinedResults.AddRange(tempResults);

            }

            //Ενημερώνουμε την στιγμή που τραβήξαμε τα δεδομένα
            App.LastLoadedDateTime = DateTime.Now;
            return combinedResults;
        }

        private List<RssModel> GetGeneralRssModels()
        {
            //Αν υπήρχαν δεδομένα από πρίν τα φρτώνουμε απο εκεί
            if (App.CurrentGeneralLoadedRssModels.Count > 0)
                return App.CurrentGeneralLoadedRssModels;

            //Ελέγχουμε ποιες ιστοσελίδες έχει επιλεγμένες ο χρήστης καθώς αν έχει μόνο οπαδικές δεν μπορούμε να εμφανίσουμε πληροφορίες

            DataView dv = App.TeamsInfoDataTable.DefaultView;
            dv.RowFilter = "teamname = 'general'"; //+ "' AND siteName IN (" + SettingsPage.SitesFilter + ")";

            DataTable generalInfoDataTable = dv.ToTable();

            //Ο πίνακας FilteredBy... έχει φιξ τιμές και δεν χρειάζεται να τον παίρνουμε σαν όρισμα, αλλά τον βάζουμε κατευθείαν μέσα στη μέθοδο
            List<RssModel> combinedResults = new List<RssModel>();

            foreach (DataRow row in generalInfoDataTable.Rows)
            {
                Device.BeginInvokeOnMainThread(() =>
                    ActivityStatusLabel.Text = "Παρακαλώ περιμένετε...\nΦόρτωση ειδήσεων από το\n" + row["siteName"]);
                Device.BeginInvokeOnMainThread(() =>
                    loadingActivitySiteImage.Source =
                        ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + row["siteName"] + ".png"));
                Device.BeginInvokeOnMainThread(GenerateRandomLoadingText);


                //Για κάθε ένα site βάζουμε τις τιμές στο combinedResults


                List<RssModel> tempResults = GetRssFeed(row["rssType"].ToString(), row["url"].ToString(),
                    row["siteName"].ToString(), SettingsPage.NumberOfRssFeedItems);
                combinedResults.AddRange(tempResults);

            }

            //Ενημερώνουμε την στιγμή που τραβήξαμε τα δεδομένα
            App.LastGeneralLoadedDateTime = DateTime.Now;
            return combinedResults;
        }

        public async void LoadDataToGrid()
        {
            //Εμφανίζουμε το stacklayout με το status φόρτωσης δεδομένων
            LoadingStatusStackLayout.IsVisible = true;
            if (GeneralNewsSelected)
            {
                App.CurrentGeneralLoadedRssModels = await Task.Run(() => GetGeneralRssModels());
                dataGrid.ItemsSource = new ObservableCollection<RssModel>(App.CurrentGeneralLoadedRssModels);
                FooterLabel.Text = "Τελευταία ενημέρωση: " + App.LastGeneralLoadedDateTime.ToString("dd/MM/yy - HH:mm");
            }
            else
            {
                App.CurrentLoadedRssModels = await Task.Run(() => GetRssModels());
                dataGrid.ItemsSource = new ObservableCollection<RssModel>(App.CurrentLoadedRssModels);
                FooterLabel.Text = "Τελευταία ενημέρωση: " + App.LastLoadedDateTime.ToString("dd/MM/yy - HH:mm");
            }
            FooterLabel.HorizontalTextAlignment = TextAlignment.Center;

            LoadingStatusStackLayout.IsVisible = false;
        }

        //Επιστρέφει τα πιο πρόσφατα feeds βάσει των τιμων που δίνονται
        public List<RssModel> GetRssFeed(string rssType, string url, string siteName, int numberOfItems)
        {
            try
            {
                //Δημιουργούμε το XDocument το οποίο είναι κοινό ανεξαιρέτως του τύπου δομής RSS/Atom
                switch (rssType)
                {
                    case "RSS":

                        XDocument rssxDocument = new XDocument();

                        //Το SDNA δεν δουλεύει χωρίς agent header οπότε αλλάζουμε τη διαδκασία λήψης του xDocument ΜΟΝΟ για το SDNA
                        if (siteName != "SDNA")
                        {
                            rssxDocument = XDocument.Load(url);
                        }
                        else
                        {
                            var request = (HttpWebRequest)WebRequest.Create(url);
                            request.UserAgent = "User-Agent: Other";

                            using (var response = request.GetResponse())
                            using (var stream = response.GetResponseStream())
                            {
                                rssxDocument = XDocument.Load(stream);
                            }
                        }

                        return (from feed in rssxDocument.Descendants("item")
                                select new RssModel
                                {
                                    SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                    Title = feed.Element("title").Value,
                                    Url = feed.Element("link").Value,
                                    PublishedDatetime = DateTime.Parse(feed.Element("pubDate").Value)
                                }).Take(numberOfItems).ToList();


                    case "Atom":
                        var atomxDocument = XDocument.Load(url);
                        return (from item in atomxDocument.Root.Elements().Where(i => i.Name.LocalName == "entry")
                                select new RssModel
                                {
                                    SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                    Url = item.Elements().First(i => i.Name.LocalName == "link").Attribute("href").Value,
                                    Title = item.Elements().First(i => i.Name.LocalName == "title").Value,
                                    PublishedDatetime =
                                        DateTime.Parse(item.Elements().First(i => i.Name.LocalName == "updated").Value)
                                }).Take(numberOfItems).ToList();
                    default:
                    case "Html":
                        int articlesfound = 0;
                        List<RssModel> rssModelFromHtml = new List<RssModel>();
                        string tempUrl = string.Empty;
                        string tempTitle = string.Empty;
                        DateTime tempPubDate = new DateTime();

                        //Εδώ έχουμε διαφορετικό σύστημα ανάλογα με την ιστοσελίδα που θα φορτώσουμε
                        switch (siteName)
                        {
                            case "SportFM":

                                #region sportFM

                                HtmlWeb sportFmWeb = new HtmlWeb();
                                HtmlDocument sportFmDocument = sportFmWeb.Load(url);

                                switch (GeneralNewsSelected)
                                {

                                    //Γενικές ειδήσεις
                                    case true:

                                        HtmlNodeCollection generalNodesCollection =
                                            sportFmDocument.DocumentNode.SelectNodes("//div[@class=\"row latest-news-flow\"]")[0].ChildNodes;

                                        foreach (var node in generalNodesCollection)
                                        {
                                            if (!node.Name.Equals("div") || !node.Attributes[0].Value.Contains("article-box")) continue;

                                            string tempdate = node.SelectSingleNode("./a/small").InnerText.Trim();
                                            rssModelFromHtml.Add(new RssModel
                                            {
                                                SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                                Url = "http://www.sport-fm.gr" + node.SelectSingleNode("./a").Attributes["href"].Value,
                                                Title = node.SelectSingleNode("./a").Attributes["title"].Value,
                                                PublishedDatetime = Convert.ToDateTime(tempdate.Substring(tempdate.Length - 17))
                                            });
                                            articlesfound++;
                                            if (articlesfound == numberOfItems) break;

                                        }


                                        break;

                                    //Ειδήσεις ομάδας
                                    default:

                                        //Το πρώτο άρθρο στο sportFM έχει άλλη κωδικοποίηση από τα υπόλοιπα
                                        HtmlNodeCollection nodesCollection =
                                            sportFmDocument.DocumentNode.SelectNodes("//div[@class=\"promo main-promo\"]");

                                        rssModelFromHtml.Add(new RssModel
                                        {
                                            SiteLogo = ImageSource.FromResource(
                                                "MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                            Url = "http://www.sport-fm.gr" + nodesCollection[0].SelectSingleNode("./a")
                                                      .Attributes["href"].Value,
                                            Title = nodesCollection[0].SelectSingleNode("./a").Attributes["title"].Value,
                                            PublishedDatetime =
                                                Convert.ToDateTime(nodesCollection[0].SelectSingleNode("./a/div/div/h3/small")
                                                    .InnerText)
                                        });

                                        articlesfound++;

                                        //Φορτώνουμε τώρα τα nodes για τα επόμενα άρθρα
                                        nodesCollection =
                                            sportFmDocument.DocumentNode.SelectNodes("//div[@class=\"col-xs-8 col-md-12 caption\"]");

                                        foreach (var node in nodesCollection)
                                        {
                                            rssModelFromHtml.Add(new RssModel
                                            {

                                                SiteLogo = ImageSource.FromResource(
                                                    "MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                                Url = "http://www.sport-fm.gr" +
                                                      node.SelectSingleNode("./h4/a[2]").Attributes["href"].Value,
                                                Title = node.SelectSingleNode("./h4/a[2]").Attributes["title"].Value,
                                                PublishedDatetime =
                                                    Convert.ToDateTime(node.SelectSingleNode("./div").InnerText.Trim())
                                            });

                                            articlesfound++;
                                            if (articlesfound == numberOfItems) break;

                                        }
                                        break;
                                }

                                break;

                            #endregion


                            case "SDNA":

                                #region SDNA

                                HtmlWeb sdnaWeb = new HtmlWeb();
                                HtmlDocument sdnaDocument = sdnaWeb.Load(url);

                                HtmlNodeCollection sdnaNodesCollection =
                                    sdnaDocument.DocumentNode.SelectNodes("//div[@class=\"view-content\"]")[2]
                                        .ChildNodes;

                                foreach (HtmlNode node in sdnaNodesCollection)
                                {
                                    if (node.Attributes["class"].Value.Equals("ad hft")) continue;

                                    //HtmlNode timeNode = node.SelectSingleNode("./div/div[2]/div[5]/div[2]/span");
                                    //HtmlNode singleNode = node.SelectSingleNode(".//div").LastChild.FirstChild.FirstChild;//.InnerHtml.Trim();
                                    //HtmlNode singleNodetest = node.SelectSingleNode("./div/div[3]/span/a");

                                    string tempDate = node.SelectSingleNode("./div/div[2]/div[5]/div[2]/span")
                                        .InnerText.Trim();

                                    if (tempDate.Contains("πριν"))
                                    {
                                        string[] splitString = tempDate.Split(new[] { " " }, StringSplitOptions.None);
                                        int secondsPassedFromPublish = 0;

                                        //Μετατρεπουμε τα δεδομένα μας σε δευτερόλεπτα και βρίσκομε την ακριβή ώρα δημοσίευσης
                                        for (int i = 0; i < splitString.Length; i++)
                                            switch (splitString[i].ToLower())
                                            {
                                                case "ώρα":
                                                case "ώρες":
                                                    secondsPassedFromPublish += int.Parse(splitString[i - 1]) * 3600;
                                                    break;
                                                case "λεπτό":
                                                case "λεπτά":
                                                    secondsPassedFromPublish += int.Parse(splitString[i - 1]) * 60;
                                                    break;
                                                case "δευτ.":
                                                    secondsPassedFromPublish += int.Parse(splitString[i - 1]);
                                                    break;
                                            }
                                        tempDate = DateTime.Now.AddSeconds(secondsPassedFromPublish * (-1))
                                            .ToString("g");
                                    }

                                    rssModelFromHtml.Add(new RssModel
                                    {
                                        SiteLogo = ImageSource.FromResource(
                                            "MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                        Url = "http://www.sdna.gr" + node.SelectSingleNode("./div/div[3]/span/a")
                                                  .Attributes["href"].Value,
                                        Title = node.SelectSingleNode("./div/div[3]/span/a").InnerText.Trim(),
                                        PublishedDatetime = Convert.ToDateTime(tempDate)
                                    });

                                    articlesfound++;

                                    if (articlesfound == numberOfItems) break;

                                }
                                break;

                            #endregion

                            case "OnSports":

                                #region onSports

                                HtmlWeb onSportsWeb = new HtmlWeb();
                                HtmlDocument onSportsdocument = onSportsWeb.Load(url);

                                HtmlNodeCollection onSportsNodesCollection =
                                    onSportsdocument.DocumentNode.SelectNodes("//div[@class=\"stories-block row\"]")[0]
                                        .ChildNodes;

                                foreach (HtmlNode node in onSportsNodesCollection)
                                {
                                    if (!node.Name.Equals("div") || node.Attributes[0].Value.Equals("clr")) continue;


                                    rssModelFromHtml.Add(new RssModel
                                    {
                                        SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                        Url = "http://www.onsports.gr" + node.SelectSingleNode("./a").Attributes["href"].Value,
                                        Title = node.SelectSingleNode("./a").InnerText,
                                        PublishedDatetime = Convert.ToDateTime(node.SelectSingleNode("./div[2]/span[2]").InnerText.Trim())
                                    });

                                    articlesfound++;

                                    if (articlesfound == numberOfItems) break;
                                }
                                break;
                            #endregion

                            case "PAOK24":

                                HtmlWeb paok24sWeb = new HtmlWeb();
                                HtmlDocument paok24Document = paok24sWeb.Load(url);

                                HtmlNodeCollection paok24NodesCollection =
                                    paok24Document.DocumentNode.SelectNodes("//div[@class=\"col-xs-12 bggray padd20T\"]")[0].ChildNodes;

                                foreach (HtmlNode node in paok24NodesCollection)
                                {
                                    //if (!node.Name.Equals("div") || node.Attributes[0].Value.Equals("clr")) continue;


                                    rssModelFromHtml.Add(new RssModel
                                    {
                                        SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                        Url = node.SelectSingleNode("./div/a").Attributes["href"].Value,
                                        Title = node.SelectSingleNode("./div/a").Attributes["title"].Value,
                                        PublishedDatetime = Convert.ToDateTime(node.SelectSingleNode("./div[2]/div[2]/p").InnerText.Trim())
                                    });

                                    articlesfound++;

                                    if (articlesfound == numberOfItems) break;
                                }

                                break;


                        }
                        return rssModelFromHtml;
                }
            }
            catch (Exception exception)
            {
                //Φτιάχνουμε μια γραμμή που ενημερώνει τον χρήστη ότι υπήρξε πρόβλημα

                return new List<RssModel>{new RssModel
                {
                    SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                    Title = "Σφάλμα λήψης άρθρων από το" + siteName +
                            "! Αν το πρόβλημα επιμένει μπορείτε πατώντας εδώ να μας στείλετε ενημερωτικό email!",
                    Url = "mailto:konstractionDev@gmail.com?subject=Σφάλμα λήψης άρθρων&body=Σφάλμα λήψης άρθρων!\n"+
                    "Ιστοσελίδα: " + siteName + Environment.NewLine+
                    "Ημερομηνία: " + DateTime.Now.ToString("yy-MM-dd HH:mm") +Environment.NewLine+
                    "Έκδοση εφαρμογής: " + DependencyService.Get<App.IGetVersionNumber>().GetVersion() + Environment.NewLine +
                    "Καταγεγραμμένο Σφάλμα: " + exception.Message,
                    PublishedDatetime = DateTime.Now
                }};
            }
        }

        //Στο κλικ του χρήστη ανοίγουμε το url που επέλεξε στον browser που χρησιμοποιεί
        private void DataGrid_OnGridTapped(object sender, GridTappedEventArgs e)
        {
            Device.OpenUri(new Uri(dataGrid.GetCellValue(e.RowData, "Url").ToString()));
        }

        // Έλεγχος σύνδεσης συσκευή στο ίντερνετ
        public bool IsDeviceConnected()
        {
            return CrossConnectivity.Current.IsConnected;
        }

        private void DataGrid_OnGridLoaded(object sender, GridLoadedEventArgs e)
        {
            if (IsDeviceConnected())
                LoadDataToGrid();
        }

        // Τυχαίο μήνυμα κατά την φόρτωση των RSS
        private void GenerateRandomLoadingText()
        {
            int randomIndex = randomNumber.Next(0, RandomMessagesArray.Length);
            LoadingRandomText.Text = RandomMessagesArray[randomIndex];
        }

        #endregion
    }
}
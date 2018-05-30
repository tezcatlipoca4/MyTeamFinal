using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MyTeam.Models;
using Plugin.Connectivity;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DataRow = System.Data.DataRow;
using System.Reflection;
using Xamarin.Forms.PlatformConfiguration;

namespace MyTeam
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RssFeedPage : ContentPage
    {

        #region Properties
        // Όλα τα τυχαία μηνύματα
        public string[] RandomMessagesArray = new string[]{
            "Ισιώνουμε τις γραμμες",
            "Κουρεύουμε το γκαζόν",
            "Ταΐζουμε τον Μπίσεσβαρ",
            "Προπονούμε τους δικαστές",
            "Πετάμε χαρταετό"

        };
		public string AppVersionNumber;
        public Random randomNumber = new Random();
        #endregion

        //Ctor
        public RssFeedPage()
        {
            InitializeComponent();

			// Get version number
			AppVersionNumber = DependencyService.Get<App.IGetVersionNumber>().GetVersion();

            //Βάζουμε τα εικονίδια στα banner από την ομάδα που έχει επιλέξει ο χρήστης
            LeftBannerTeamLogo.Source = RightBannerTeamLogo.Source =
            ImageSource.FromResource("MyTeam.Assets.Images.teamLogos." + SettingsPage.TeamChosen + ".png");
            teamLabel.Text = SettingsPage.TeamLabel;

            dataGrid.GridLoaded += DataGrid_OnGridLoaded;


            pullToRefresh.Refreshing += PullToRefresh_Refreshing;


            pullToRefresh.Refreshing += PullToRefresh_Refreshing;

            dataGrid.GroupCaptionTextFormat = "Δημοσιεύθηκαν: {Key}";
            dataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription()
            {
                ColumnName = "PublishedDatetime",
                Converter = new GroupConverterByDate()

            });

        }

        #region Methods
        private void PullToRefresh_Refreshing(object sender, EventArgs e)
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
                Device.BeginInvokeOnMainThread(() => loadingActivitySiteImage.Source = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + row["siteName"] + ".png"));
                Device.BeginInvokeOnMainThread(GenerateRandomLoadingText);


                //Για κάθε ένα site βάζουμε τις τιμές στο combinedResults
                //TODO: Να επιλέγει ο χρήστης από 5-15 άρθρα από κάθε σελίδα
                List<RssModel> tempResults = GetRssFeed(row["rssType"].ToString(), row["url"].ToString(),
                    row["siteName"].ToString(), SettingsPage.NumberOfRssFeedItems);
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
			FooterLabel.Text = "Τελευταία ενημέρωση: " + App.LastLoadedDateTime.ToString("dd/MM/yy - HH:mm") + 
				" | Έκδοση: " + AppVersionNumber;
				
            FooterLabel.HorizontalTextAlignment = TextAlignment.Center;

            LoadingStatusStackLayout.IsVisible = false;
        }

        //Επιστρέφει τα πιο πρόσφατα feeds βάσει των τιμων που δίνονται
        public List<RssModel> GetRssFeed(string rssType, string url, string siteName, int numberOfItems)
        {
            //todo:Αν το feed δεν ερχεται σωστα να εημερώνεται ο χρήστης και να αφαιρείται 

            //Δημιουργούμε το XDocument το οποίο είναι κοινό ανεξαιρέτως του τύπου δομής RSS/Atom
            switch (rssType)
            {

                case "RSS":
                    var rssxDocument = XDocument.Load(url);
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
                    List<RssModel> rssModelFromHtml = new List<RssModel>();
                    string tempUrl = string.Empty;
                    string tempTitle = string.Empty;
                    DateTime tempPubDate = new DateTime();
                    //Εδώ έχουμε διαφορετικό σύστημα ανάλογα με την ιστοσελίδα που θα φορτώσουμε
                    switch (siteName)
                    {
                        case ("SportFM"):
                            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                            {
                                client.Encoding = Encoding.UTF8;
                                string htmlCode = client.DownloadString(url);

                                StringReader reader = new StringReader(htmlCode);
                                string line;
                                int articlesFound = 0;
                                bool foundStart = false;

                                while ((line = reader.ReadLine()) != null)
                                {
                                    //Μέχρι να συναντήσουμε το <div class="row"> δεν κρατάμε τπτ
                                    if (line.Contains("<div class=\"row\">"))
                                        foundStart = true;

                                    if (!foundStart) continue;

                                    if (line.Contains("<a href=\"/article/") && line.Contains("title") && !line.Contains("</a>"))
                                    {
                                        line = line.Trim().Replace("<a href=\"", string.Empty);

                                        int endUrlQuotreIndex = line.IndexOf('\"');

                                        tempUrl = "http://www.sport-fm.gr" + line.Substring(0, endUrlQuotreIndex - 1);
                                        //richTextBox1.AppendText("http://www.sport-fm.gr" + line.Substring(0, endUrlQuotreIndex - 1) + Environment.NewLine);

                                        //Αφαιρούμε πλέον το url από τη γραμμή, το πρόθεμα του τίτλου και τα σύμβολα στο τέλος
                                        line = line.Remove(0, endUrlQuotreIndex).Replace("\" title=\"", string.Empty).Replace("\">", string.Empty);

                                        tempTitle = line;
                                        //richTextBox1.AppendText(line + Environment.NewLine);
                                        articlesFound++;
                                    }
                                    else if (line.Contains("<span>"))

                                    {
                                        tempPubDate = Convert.ToDateTime(line.Replace("<span>", "").Replace("</span>", "").TrimStart());

                                        //Πλέον έχουμε όλα τα στοιχεία που χρειαζόμαστε, προσθέτουμε στο RSSModel

                                        rssModelFromHtml.Add(new RssModel
                                        {
                                            SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                            Url = tempUrl,
                                            Title = tempTitle,
                                            PublishedDatetime = tempPubDate
                                        });

                                        //richTextBox1.AppendText(line.Replace("<span>", "").Replace("</span>", "").TrimStart() + "\n");
                                        if (articlesFound >= numberOfItems) break;
                                    }
                                    //Μόνο για το πρώτο άρθρο που έχει διαφορετική ρύθμιση για την ώρα!
                                    else if (line.Contains("article-date") && articlesFound == 1)
                                    {
                                        tempPubDate = Convert.ToDateTime(line.Remove(0, line.IndexOf("\">") + 2).Replace("</small></h3>", string.Empty));

                                        rssModelFromHtml.Add(new RssModel
                                        {
                                            SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                            Url = tempUrl,
                                            Title = tempTitle,
                                            PublishedDatetime = tempPubDate
                                        });

                                    }
                                }
                            }
                            break;

                        case ("SDNA"):
                            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                            {
                                client.Encoding = Encoding.UTF8;
                                client.Headers.Add("User-Agent: Other");

                                //Το SDNA έχει όλες τις πληροφορίες σε μια γραμμή! Την εντοπίζουμε και την σπάμε σε σειρές ώστε να μπορέσουμε να πάρουμε τα άρθρα
                                bool foundStart = false;
                                string htmlCode = client.DownloadString(url).Replace("><", ">\n<");
                                StringReader reader = new StringReader(htmlCode);
                                int articlesFound = 0;
                                string line;

                                while ((line = reader.ReadLine()) != null)
                                {
                                    //Μέχρι να συναντήσουμε το <div class="row"> που είναι και η μοναδική γραμμή που θέλουμε
                                    if (line.Contains("<div class=\"external-wrapper\">"))
                                        foundStart = true;

                                    if (!foundStart) continue;

                                    //Έλεγχος για τις δυο περιπτώσεις ημερομηνίας
                                    if (line.Contains("<em class=\"placeholder\">"))
                                    {
                                        //Το string περιέχει δεδομένα όπως 1 ωρα 11 λεπτά. Το μετατρέπουμε σε κανονική ημερομηνία
                                        line = line.Replace("<span class=\"field-content\"> <em class=\"placeholder\">", "").Replace("</em> πριν </span>", "").Trim();

                                        string[] splitString = line.Split(new[] { " " }, StringSplitOptions.None);
                                        int secondsPassedFromPublish = 0;

                                        //Μετατρεπουμε τα δεδομένα μας σε δευτερόλεπτα και βρίσκομε την ακριβή ώρα δημοσίευσης
                                        for (int i = 0; i < splitString.Length; i++)
                                        {
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
                                        }

                                        tempPubDate = DateTime.Now.AddSeconds(secondsPassedFromPublish * (-1));

                                    }

                                    else if (line.Contains("<span class=\"field-content\">") && line.Contains("</span>"))
                                    {
                                        //Η σειρά μας ενδιαφέρει μόνο αν έχει ημερομηνία μέσα
                                        string[] format = { "dd MMMM yyyy, HH:mm" };
                                        line = line.Replace("<span class=\"field-content\">", "").Replace("</span>", "").TrimStart().TrimEnd();
                                        DateTime retrievedDateTime;

                                        if (DateTime.TryParseExact(line, format,
                                            new CultureInfo("el-GR"),
                                            //CultureInfo.CurrentCulture,
                                            DateTimeStyles.AssumeLocal, out retrievedDateTime))
                                            tempPubDate = retrievedDateTime;

                                    }
                                    else if (!line.Contains("div class") && line.Contains("<a href=\"/"))
                                    {
                                        line = line.Replace("<a href=\"", string.Empty).Replace("</a>", string.Empty);

                                        //Το σύμβολο (") χωρίζει το url από τον τίτλο
                                        int breakSymbolIndex = line.IndexOf('\"');
                                        string tempUrlLine = line.Substring(0, breakSymbolIndex);
                                        tempUrl = "http://www.sdna.gr" + tempUrlLine;
                                        //richTextBox1.AppendText("www.sdna.gr" + url + Environment.NewLine);

                                        //Αφαιρούμε το url και τα διαχωριστικά (">) και μένει μόνο ο τίτλος του άρθρου
                                        tempTitle = line.Replace(tempUrlLine, string.Empty).Replace("\">", string.Empty);


                                        //Έχουμε όλα τα στοιχεία που χρειαζόμαστε, προσθέτουμε στα RSSModels

                                        rssModelFromHtml.Add(new RssModel
                                        {
                                            SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                            Url = tempUrl,
                                            Title = tempTitle,
                                            PublishedDatetime = tempPubDate
                                        });
                                        articlesFound++;

                                        if (articlesFound >= numberOfItems) break;
                                    }

                                }

                            }
                            break;
                        case ("OnSports"):
                            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                            {
                                client.Encoding = Encoding.UTF8;
                                client.Headers.Add("User-Agent: Other");
                                string htmlCode = client.DownloadString(url).Replace("<span class=\"story-date\">" + "\n", "<span class=\"story-date\">");//.Replace("><",">\n<");
                                StringReader reader = new StringReader(htmlCode);
                                int articlesFound = 0;
                                string line;

                                bool foundStart = false;

                                while ((line = reader.ReadLine()) != null)
                                {
                                    //Μέχρι να συναντήσουμε το <div class="row"> που είναι και η μοναδική γραμμή που θέλουμε
                                    if (line.Contains("<div class=\"stories-block row\">"))
                                        foundStart = true;

                                    if (!foundStart) continue;

                                    //Ημερομηνία
                                    if (line.Contains("<span class=\"story-date\">"))
                                    {
                                        line = line.Replace("<span class=\"story-date\">", string.Empty).Replace("</span>", string.Empty).Trim();
                                        //Γίνεται γιατί οι τσομπάνηδες έχουν τον Μάιο χωρίς διαλυτικά

                                        line = line.Replace("ί", "ΐ");

                                        string[] format = { "dd MMMM yyyy, HH:mm" };
                                        DateTime retrievedDateTime;

                                        if (DateTime.TryParseExact(line, format,
                                            new CultureInfo("el-GR"),
                                            //CultureInfo.CurrentCulture,
                                            DateTimeStyles.AssumeLocal, out retrievedDateTime))
                                            
                                            tempPubDate = retrievedDateTime;

                                    }
                                    else if (!line.Contains("div class") && line.Contains("<a href=\"/"))
                                    {
                                        line = line.Replace("<a href=\"", string.Empty).Replace("</a>", string.Empty).Trim();

                                        //Το σύμβολο (") χωρίζει το url από τον τίτλο
                                        int breakSymbolIndex = line.IndexOf('\"');
                                        string tempLineUrl = line.Substring(0, breakSymbolIndex);
                                        tempUrl  = "http://www.onsports.gr" + tempLineUrl;

                                        //Αφαιρούμε το url και τα διαχωριστικά (">) και μένει μόνο ο τίτλος του άρθρου
                                        tempTitle = line.Replace(tempLineUrl, string.Empty).Replace("\">", string.Empty);

                                        rssModelFromHtml.Add(new RssModel
                                        {
                                            SiteLogo = ImageSource.FromResource("MyTeam.Assets.Images.siteLogos." + siteName + ".png"),
                                            Url = tempUrl,
                                            Title = tempTitle,
                                            PublishedDatetime = tempPubDate
                                        });

                                        articlesFound++;

                                    }

                                    if (articlesFound >= numberOfItems) break;
                                }
                            }
                            break;

                    }
                    return rssModelFromHtml;

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
            {
                LoadDataToGrid();
            }
        }

        // Τυχαίο μήνυμα κατά την φόρτωση των RSS
        void GenerateRandomLoadingText()
        {
            int randomIndex = randomNumber.Next(0, RandomMessagesArray.Length);
            LoadingRandomText.Text = RandomMessagesArray[randomIndex];
        }
        #endregion


    }
}
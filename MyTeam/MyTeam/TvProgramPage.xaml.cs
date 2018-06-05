using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using MyTeam.Models;
using Xamarin.Forms;
using Syncfusion.Data.Extensions;

namespace MyTeam
{
	public partial class TvProgramPage : ContentPage
	{
		string numberDate = string.Empty;
        string monthYear = string.Empty;
    	string textDate = string.Empty;

        // Ctor
		public TvProgramPage()
		{
			InitializeComponent();

			HtmlWeb web = new HtmlWeb();
			HtmlDocument document = web.Load("http://www.sdna.gr/tv-program");

			GetSetFullDate(document);

			// Get Schedule nodes
			HtmlNodeCollection scheduleTimeNodes = document.DocumentNode.SelectNodes("//div[@class=\"time-based\"]");

			// Save Schedules in this list
			List<Schedule> allEventItems = new List<Schedule>();

			// Set Schedules
			SetChedules(scheduleTimeNodes, allEventItems);
            
            // Bind to Xaml List
			scheduleList.ItemsSource = allEventItems;

            // Disable selected item 
			scheduleList.ItemSelected += (object sender, SelectedItemChangedEventArgs args) =>
                {
                    ((ListView)sender).SelectedItem = null;
                };

		}

        // Methods
		void SetChedules(HtmlNodeCollection scheduleTimeNodes, List<Schedule> allEventItems)
		{
			foreach (HtmlNode node in scheduleTimeNodes)
			{
				// Set time                
				string _time = node.SelectSingleNode("./div").InnerText.Trim();

				HtmlNodeCollection entries = node.SelectNodes("./div[2]");
                
				foreach (var entry in entries)
				{					
					HtmlNode infoDiv = entry.SelectSingleNode("./div");
					HtmlNode titleDiv = infoDiv.SelectSingleNode("./div");

					string _title = titleDiv.FirstChild.InnerText.Trim();
					string _channel = titleDiv.SelectSingleNode("./div[3]").InnerText.Trim();
					string _desc = titleDiv.SelectSingleNode("./div[4]").InnerText.Trim();

					Schedule scheduleItem = new Schedule
					{
						Title = _title,
						NumberDate = numberDate,
						MonthYear = monthYear,
						Time = _time,
						Channel = _channel,
						Description = _desc                            
					};

					allEventItems.Add(scheduleItem);
				}
			}
		}
        
		void GetSetFullDate(HtmlDocument document)
		{

			// Get full Date
			HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//div[@class=\"date\"]");

			// Set full date
			foreach (HtmlNode node in nodes)
			{

				numberDate = node.SelectSingleNode("./span").InnerText.Trim();
				monthYear = node.SelectSingleNode("./span[2]").InnerText.Trim();
				textDate = node.SelectSingleNode("./span[3]").InnerText.Trim();
			}
		}

        
	}                  
}

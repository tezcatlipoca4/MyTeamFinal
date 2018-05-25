using MyTeam.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MyTeam
{
    class GroupConverterByDate : IValueConverter
    {


        public GroupConverterByDate()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((RssModel)value).PublishedDatetime.ToString("d MMM yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}

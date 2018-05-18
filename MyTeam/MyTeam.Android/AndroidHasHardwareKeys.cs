using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyTeam.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidHasHardwareKeys))]
namespace MyTeam.Droid
{
    class AndroidHasHardwareKeys : App.IHasHardwareKeys
    { 
        public bool GetHardwareKeys()
        {
            return ViewConfiguration.Get(Android.App.Application.Context).HasPermanentMenuKey;
        }
    }
}
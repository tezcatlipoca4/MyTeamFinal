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
        public bool IsNavigationBarAvailable()
        {

            bool hasBackKey = KeyCharacterMap.DeviceHasKey(KeyEvent.KeyCodeFromString("KEYCODE_BACK"));
            bool hasHomeKey = KeyCharacterMap.DeviceHasKey(KeyEvent.KeyCodeFromString("KEYCODE_HOME"));

            if (hasBackKey && hasHomeKey)
            {
                // no navigation bar, unless it is enabled in the settings
                return false;
            }
            else
            {
                // 99% sure there's a navigation bar
                return true;
            }            
        }
    }
}
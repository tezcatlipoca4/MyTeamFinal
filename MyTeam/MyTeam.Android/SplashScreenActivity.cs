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

namespace MyTeam.Droid
{
    [Activity(
        Theme = "@style/splashTheme",
        MainLauncher = true,
        NoHistory = true
        )
        ]

    public class SplashScreenActivity : Activity
    {
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(MainActivity));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            //StartActivity(typeof(MainActivity));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MyTeam;
using MyTeam.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobRenderer))]
namespace MyTeam.Droid
{
    public class AdMobRenderer : ViewRenderer
    {
        public AdMobRenderer(Context context) : base(context)
        {

        }

        private int GetSmartBannerDpHeight()
        {
            var dpHeight = Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density;

            if (dpHeight <= 400) return 32;
            if (dpHeight > 400 && dpHeight <= 720) return 50;
            return 90;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var ad = new AdView(Context)
                {
                    AdSize = AdSize.Banner,
					//AdUnitId = "ca-app-pub-2827816165257335/5822286130" // Real Ads
                    AdUnitId = "ca-app-pub-3940256099942544/6300978111" // Test Ads
                };

                var requestbuilder = new AdRequest.Builder();

                ad.LoadAd(requestbuilder.Build());
                e.NewElement.HeightRequest = GetSmartBannerDpHeight();

                SetNativeControl(ad);
            }
        }
    }
}
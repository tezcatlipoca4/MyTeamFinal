using System;
using System.Runtime.Remoting.Messaging;
using Android.Support.V7.Util;
using MyTeam.Droid;
using Xamarin.Forms;
using Android.Test.Mock;
using Android.Content.PM;

[assembly: Dependency(typeof(AndroidVersion))]
namespace MyTeam.Droid
{
	class AndroidVersion : App.IGetVersionNumber
	{
		public string GetVersion()
		{
			var context = Android.App.Application.Context;
			return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;

		}
	}
}

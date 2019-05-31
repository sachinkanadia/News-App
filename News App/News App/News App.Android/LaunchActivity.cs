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

namespace News_App.Droid
{
    [Activity(
             Label = "News App",
             Theme = "@style/Theme.Splash", //Indicates the theme to use for this activity
             MainLauncher = true, //Set it as boot activity
             NoHistory = true)] //Doesn't place it in back stack
    public class LaunchActivity : Activity
    {
        //Need to move to main activity.
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            StartActivity(typeof(MainActivity));
        }
    }
}
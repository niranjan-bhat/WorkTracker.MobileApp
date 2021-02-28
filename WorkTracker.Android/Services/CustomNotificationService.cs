using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WorkTracker.Contracts;
using WorkTracker.Droid.Services;

[assembly: Xamarin.Forms.Dependency(typeof(CustomNotificationService))]
namespace WorkTracker.Droid.Services
{
    public class CustomNotificationService
    {
        public void Notify(string message)
        {
            Context context = Android.App.Application.Context;
            var toast = Toast.MakeText(context, message, ToastLength.Short);
            toast.SetGravity(GravityFlags.Top | GravityFlags.FillHorizontal, 0, 100);

            //var inflater = Application.Context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
            //var notificationView = inflater.Inflate(Resource.Layout.CustomNotificationLayout, null);
            //TextView textView = notificationView.FindViewById<TextView>(Resource.Id.txtMessage);
            //textView.Text = message;

            //toast.View = notificationView;

            toast.Show();
        }
    }
}
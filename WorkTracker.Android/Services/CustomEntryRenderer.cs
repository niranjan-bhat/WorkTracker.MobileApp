using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content.Res;
using Android.Graphics;
using WorkTracker.Droid.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;
using WorkTracker.CustomControls;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace WorkTracker.Droid.Services
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context con) : base(con)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            Control.SetTextCursorDrawable(Resource.Drawable.EntryCursor);

            if (Control == null || e.NewElement == null) return;

            if (e.NewElement is CustomEntry _entry)
            {

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                    Control.BackgroundTintList = ColorStateList.ValueOf(_entry.UnderlineColor.ToAndroid());
                else
                    Control.Background.SetColorFilter(_entry.UnderlineColor.ToAndroid(), PorterDuff.Mode.SrcAtop);
            }
        }
    }
}
using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Widget;
using WorkTracker.CustomControls;
using WorkTracker.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RoundEdgeEntryBox), typeof(RoundEdgeEntryBoxRenderer))]

namespace WorkTracker.Droid.Renderers
{
    public class RoundEdgeEntryBoxRenderer : EntryRenderer
    {
        public RoundEdgeEntryBoxRenderer(Context context) : base(context)
        {
        }

        public RoundEdgeEntryBox ElementV2 => Element as RoundEdgeEntryBox;
        protected override FormsEditText CreateNativeControl()
        {
            var control = base.CreateNativeControl();
            UpdateBackground(control);
            return control;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            Control.SetTextCursorDrawable(Resource.Drawable.EntryCursor);
            Control.SetTextIsSelectable(false);
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == RoundEdgeEntryBox.CornerRadiusProperty.PropertyName)
            {
                UpdateBackground();
            }
            else if (e.PropertyName == RoundEdgeEntryBox.BorderThicknessProperty.PropertyName)
            {
                UpdateBackground();
            }
            else if (e.PropertyName == RoundEdgeEntryBox.BorderColorProperty.PropertyName)
            {
                UpdateBackground();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        protected override void UpdateBackgroundColor()
        {
            UpdateBackground();
        }
        protected void UpdateBackground(FormsEditText control)
        {
            if (control == null) return;

            var gd = new GradientDrawable();
            gd.SetColor(Element.BackgroundColor.ToAndroid());
            gd.SetCornerRadius(Context.ToPixels(ElementV2.CornerRadius));
            gd.SetStroke((int)Context.ToPixels(ElementV2.BorderThickness), ElementV2.BorderColor.ToAndroid());
            control.SetBackground(gd);

            var padTop = (int)Context.ToPixels(ElementV2.Padding.Top);
            var padBottom = (int)Context.ToPixels(ElementV2.Padding.Bottom);
            var padLeft = (int)Context.ToPixels(ElementV2.Padding.Left);
            var padRight = (int)Context.ToPixels(ElementV2.Padding.Right);

            control.SetPadding(padLeft, padTop, padRight, padBottom);
        }
        protected void UpdateBackground()
        {
            UpdateBackground(Control);
        }
    }
}
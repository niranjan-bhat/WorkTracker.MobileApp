using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace WorkTracker.CustomControls.Behavior
{
    public class EntryRestrictionOnlyAlphabet : Behavior<Entry>
    {

        const string alphabetRegex = @"^[a-zA-Z ]+$";

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(EntryRestrictionOnlyAlphabet), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
        }

        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(e.NewTextValue))
                return;

            IsValid = (Regex.IsMatch(e.NewTextValue, alphabetRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));
            ((Entry)sender).Text = IsValid ? e.NewTextValue : e.OldTextValue;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
        }
    }
}


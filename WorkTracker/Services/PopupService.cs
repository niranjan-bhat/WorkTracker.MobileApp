using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using WorkTracker.Classes;
using WorkTracker.Contracts;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorkTracker.Services
{
    public class PopupService : IPopupService
    {
        public void HideLoadingScreen()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.HideLoading();
            });
        }

        public void ShowLoadingScreen()
        { 
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.ShowLoading("Loading", null);
            });
        }

        public async Task<bool> ShowPopup(string title, string message, string yesButtonText, string noButtonText)
        {
            var config = new ConfirmConfig()
            {
                CancelText = noButtonText,
                Message = message,
                OkText = yesButtonText,
                Title = title,
            };
            bool result = false;

            await MainThread.InvokeOnMainThreadAsync(async () =>
             {
                 result = await UserDialogs.Instance.ConfirmAsync(config, null);
             });
            return result;
        }


    }
}

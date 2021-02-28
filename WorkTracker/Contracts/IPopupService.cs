using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkTracker.Contracts
{
    public interface IPopupService
    {
        Task<bool> ShowPopup(string title, string message, string yesButtonText, string noButtonText);
        void ShowLoadingScreen();
        void HideLoadingScreen();
    }
}

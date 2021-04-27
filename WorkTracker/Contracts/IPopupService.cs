using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkTracker.Contracts
{
    public interface IPopupService
    {
        /// <summary>
        /// Shows a Yes/No popup
        /// </summary>
        /// <param name="title">Heading of the popup</param>
        /// <param name="message">Message to show</param>
        /// <param name="yesButtonText">Yes button's text</param>
        /// <param name="noButtonText">No button's text</param>
        /// <returns></returns>
        Task<bool> ShowPopup(string title, string message, string yesButtonText, string noButtonText);

        /// <summary>
        /// Shows a loading screen
        /// </summary>
        void ShowLoadingScreen();

        /// <summary>
        /// Hides a loading screen 
        /// </summary>
        void HideLoadingScreen();
    }
}

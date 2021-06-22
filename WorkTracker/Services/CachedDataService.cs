using System;
using Newtonsoft.Json;
using WorkTracker.Classes;
using WorkTracker.Database.DTO;
using Xamarin.Essentials;

namespace WorkTracker.Services
{
    public class CachedDataService : ICachedDataService
    {
        private string _email;

        public OwnerDTO GetCachedOwner()
        {
            if (Preferences.ContainsKey(_email))
            {
                var storedDataAsString = Preferences.Get(_email, string.Empty);
                return JsonConvert.DeserializeObject<OwnerDTO>(storedDataAsString);
            }

            return null;
        }

        public DateTime GetLastSubmittedDate()
        {
            if (Preferences.ContainsKey(_email + Constants.LatestDateOfAttendanceSubmission))
            {
                var dateTime = Preferences.Get(_email + Constants.LatestDateOfAttendanceSubmission, DateTime.Today);
                return dateTime;
            }

            return DateTime.Today;
        }

        public void UpdateLastSubmittedDate(DateTime date)
        {
            Preferences.Set(_email + Constants.LatestDateOfAttendanceSubmission, date);
        }

        public void CacheOwner(OwnerDTO owner)
        {
            _email = owner.Email;

            var cachedString = JsonConvert.SerializeObject(owner);
            Preferences.Set(_email, cachedString);
        }

    }

    public interface ICachedDataService
    {
        public OwnerDTO GetCachedOwner();
        public DateTime GetLastSubmittedDate();
        public void UpdateLastSubmittedDate(DateTime date);
        public void CacheOwner(OwnerDTO owner);
    }
}

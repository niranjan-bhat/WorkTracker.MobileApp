using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WorkTracker
{
    public class Constants
    {
        public const string DatabaseFilename = "WorkTracker.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }

        #region NavigationPages

        public const string Navigation = "Navigation";
        public const string AddWorkerPage = "AddWorkerPage";
        public const string MainPage = "MainPage";
        public const string SummaryPage = "SummaryPage";
        public const string JobAssignmentPage = "JobAssignmentPage";
        public const string Login = "LoginPage";
        public const string SignUpPage = "SignUpPage";

        #endregion
        public const string UserEmail = "UserEmail";
        public const string UserId = "UserId";
        public const string DateFormat = "yyyy-MM-dd";

    }
}

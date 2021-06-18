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
        public const string JobView = "JobView";
        public const string Login = "LoginPage";
        public const string SignUpPage = "SignUpPage";
        public const string WorkerSalaryPage = "WorkerSalaryView";
        public const string WorkerPaymentPage = "PaymentView";
        public const string JobStatistics = "JobStatistics";

        #endregion
        public const string UserEmail = "UserEmail";
        public const string UserId = "UserId";
        public const string DateFormat = "yyyy-MM-dd";
        public const string LatestDateOfAttendanceSubmission = "LatestDateOfAttendanceSubmission";
        public const string Job = "Job";

        #region ErrorCodes
        public static string OWNER_NOT_FOUND = "OWNER_NOT_FOUND";
        public static string WORKER_NOT_FOUND = "WORKER_NOT_FOUND";
        public static string JOB_NOT_FOUND = "JOB_NOT_FOUND";
        public static string ASSIGNMENT_NOT_FOUND = "ASSIGNMENT_NOT_FOUND";
        public static string INVALID_TOKEN = "INVALID_TOKEN";
        public static string USERNAMEPASSWORD_WRONG = "USERNAMEPASSWORD_WRONG";
        public static string DUPLICATE_JOBNAME = "DUPLICATE_JOBNAME";
        public static string DUPLICATE_MOBILE_NUMBER = "DUPLICATE_MOBILE_NUMBER";
        public static string INVALID_EMAIL = "INVALID_EMAIL";
        internal static string DUPLICATE_WORKERNAME = "DUPLICATE_WORKERNAME";
        public static string DUPLICATE_EMAIL = "DUPLICATE_EMAIL";
        #endregion

    }
}

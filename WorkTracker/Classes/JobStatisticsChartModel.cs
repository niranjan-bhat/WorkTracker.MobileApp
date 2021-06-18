using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace WorkTracker.Classes
{
    public class JobStatisticsChartModel
    {
        public string WorkerName { get; set; }
        public int Days { get; set; }
        public Color ColorCode { get; set; }
        public int? TotalWage { get; set; }
    }
}

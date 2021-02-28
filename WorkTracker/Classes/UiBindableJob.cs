using WorkTracker.Database.DTOs;

namespace WorkTracker.Classes
{
    public class UiBindableJob : SelectableItem
    {
        public JobDTO Job { get; set; }

        public UiBindableJob()
        {
            Job = new JobDTO();
        }
    }
}

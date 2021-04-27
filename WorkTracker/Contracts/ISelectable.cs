namespace WorkTracker.Contracts
{
    public interface ISelectable
    {
        /// <summary>
        /// Boolean to check if it is selectable
        /// </summary>
        bool IsSelected { get; set; }
    }
}
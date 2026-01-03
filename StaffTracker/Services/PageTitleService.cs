namespace StaffTracker.Services
{
    public interface IPageTitleService
    {
        string TitleKey { get; }
        event Action? OnChange;
        void SetTitle(string titleKey);
    }

    public class PageTitleService : IPageTitleService
    {
        public string TitleKey { get; private set; } = "AppTitle";
        public event Action? OnChange;

        public void SetTitle(string titleKey)
        {
            TitleKey = titleKey;
            OnChange?.Invoke();
        }
    }
}

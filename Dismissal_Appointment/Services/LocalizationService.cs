using Dismissal_Appointment.Resources.Translations;
using System.Globalization;

namespace Dismissal_Appointment.Services
{
    public interface ILocalizationService
    {
        string this[string key] { get; }
        string this[string key, params object[] arguments] { get; }
        
        event Action? OnCultureChanged;
        void SetCulture(string culture);
    }

    public class LocalizationService : ILocalizationService
    {
        public event Action? OnCultureChanged;

        public string this[string key]
        {
            get
            {
                var value = SharedResource.ResourceManager.GetString(key, CultureInfo.CurrentUICulture);
                return value ?? key;
            }
        }

        public string this[string key, params object[] arguments]
        {
            get
            {
                var format = SharedResource.ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
                return string.Format(format, arguments);
            }
        }

        public void SetCulture(string culture)
        {
            var cultureInfo = new CultureInfo(culture);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            OnCultureChanged?.Invoke();
        }
    }
}

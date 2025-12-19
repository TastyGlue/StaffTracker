using Dismissal_Appointment.Resources.Translations;
using System.Globalization;

namespace Dismissal_Appointment.Services
{
    public interface ILocalizationService
    {
        string this[string key] { get; }
        string this[string key, params object[] arguments] { get; }
        CultureInfo CurrentCulture { get; }
    }

    public class LocalizationService : ILocalizationService
    {
        public CultureInfo CurrentCulture => CultureInfo.CurrentUICulture;

        public string this[string key]
        {
            get
            {
                var value = SharedResource.ResourceManager.GetString(key, CurrentCulture);
                return value ?? key;
            }
        }

        public string this[string key, params object[] arguments]
        {
            get
            {
                var format = SharedResource.ResourceManager.GetString(key, CurrentCulture) ?? key;
                return string.Format(format, arguments);
            }
        }
    }
}

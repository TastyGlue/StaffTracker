using StaffTracker.Resources.Translations;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace StaffTracker.Services;

public class ResXMudLocalizer : MudLocalizer
{
    public override LocalizedString this[string key]
    {
        get
        {
            var value = MudBlazorResource.ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
            return new LocalizedString(key, value);
        }
    }

    public override LocalizedString this[string key, params object[] arguments]
    {
        get
        {
            var format = MudBlazorResource.ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
            return new LocalizedString(key, string.Format(format, arguments));
        }
    }
}

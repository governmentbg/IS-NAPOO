using Microsoft.Extensions.Localization;

namespace ISNAPOO.WebSystem.Resources
{
    public interface ILocService
    {
        LocalizedString GetLocalizedHtmlString(string key);
    }
}
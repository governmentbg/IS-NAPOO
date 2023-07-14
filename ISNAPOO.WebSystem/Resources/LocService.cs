using Microsoft.Extensions.Localization;
using System.Reflection;

namespace ISNAPOO.WebSystem.Resources
{
    public class LocService : ILocService
    {
        private readonly IStringLocalizer _localizer;

        public LocService(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResource", assemblyName.Name);
        }

        public LocalizedString GetLocalizedHtmlString(string key)
        {


            LocalizedString result = _localizer[key];

            //if (_localizer[key] == key)
            //{
            //    result = new LocalizedString(key, "*" + key + "*");
            //}

            return result;
        }
    }
}

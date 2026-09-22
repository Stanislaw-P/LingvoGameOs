using System.Globalization;
using System.Resources;

namespace LingvoGameOs.Helpers
{
    public class SecondLocalizer
    {
        private readonly ResourceManager _rm;
        private readonly IHttpContextAccessor _http;

        public SecondLocalizer(IHttpContextAccessor http)
        {
            _http = http;
            _rm = new ResourceManager(
                "LingvoGameOs.Resources.SharedResource",
                typeof(SharedResource).Assembly);
        }

        public string this[string key]
        {
            get
            {
                var lang = _http.HttpContext?.Request.Cookies["SecondLanguage"];
                if (string.IsNullOrEmpty(lang))
                    lang = "ru";

                return _rm.GetString(key, new CultureInfo(lang)) ?? key;
            }
        }
    }
}

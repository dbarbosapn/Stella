using System.Globalization;
using System.Threading;

namespace Stella.Helper
{
    public class LocaleHelper
    {
        public static LocaleHelper Instance {
            get {
                if (_instance == null)
                    _instance = new LocaleHelper();
                return _instance;
            }
        }
        private static LocaleHelper _instance;

        /// <summary>
        /// Loads locale settings
        /// </summary>
        public void Load()
        {
            var locale = Properties.Settings.Default.Locale;
            CultureInfo info;

            try
            {
                info = new CultureInfo(locale);
            }
            catch (CultureNotFoundException)
            {
                info = new CultureInfo("en");
            }

            Thread.CurrentThread.CurrentUICulture = info;
            Thread.CurrentThread.CurrentCulture = info;
        }

        /// <summary>
        /// Sets and saves the new given locale
        /// </summary>
        /// <param name="locale">Given locale to set</param>
        /// <returns>Boolean representing if the property was changed</returns>
        public bool SetLocale(string locale)
        {
            if(Properties.Settings.Default.Locale != locale)
            {
                Properties.Settings.Default.Locale = locale;
                Properties.Settings.Default.Save();
                Load();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the currently set locale
        /// </summary>
        /// <returns>Currently set locale as string</returns>
        public string GetCurrentLocale()
        {
            return Properties.Settings.Default.Locale;
        }
    }
}

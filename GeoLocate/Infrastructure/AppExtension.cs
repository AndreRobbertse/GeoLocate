using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoLocate
{
    public static class AppExtension
    {
        /// <summary>
        /// Change the number Decimal Seperator
        /// E.g. comma "," or point "."
        /// </summary>
        /// <param name="seperator"></param>
        public static void NumberDecimalSeparatorChange(string seperator)
        {
            if (!string.IsNullOrEmpty(seperator) && seperator.Length == 1)
            {
                // Force NumberDecimalSeparator to point for Coord parsing
                System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
                customCulture.NumberFormat.NumberDecimalSeparator = seperator;
                System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            }
        }
    }
}

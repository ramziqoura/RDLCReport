using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSoft.MethodExtension
{
    public static class DateTimeExtention
    {
        /// <summary>
        /// Permet d'obternir un DateTime ne comportant que la date.
        /// </summary>        
        public static DateTime ToShortDateTime(this DateTime s)
        {
            return DateTime.Parse(s.ToShortDateString());
        }

        /// <summary>
        /// Méthode d'extension pour trouver la première journée de la semaine
        /// </summary>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
        
    }
}

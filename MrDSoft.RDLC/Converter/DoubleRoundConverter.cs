﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using DSoft.MethodExtension;
using System.ComponentModel;


namespace DSoft.Converter
{
    /// <summary>
    /// Arrondir un nombre avec une quantité paramétrable de digits après la virgule.
    /// Retourne un double
    /// Le nombre de digit est 2 par défaut
    /// </summary>
    public class DoubleRoundConverter : IValueConverter
    {
        private int _precision;

    
        [DefaultValue(2)]
        public int Precision
        {
            get
            {
                return _precision;
            }
            set
            {
                _precision = value;
            }
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = double.NaN;

            try
            {
                val = (double)value;
            }
            catch { }


            if (val != double.NaN)
                return val.Round(_precision);
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

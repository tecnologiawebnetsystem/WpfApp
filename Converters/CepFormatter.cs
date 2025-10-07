using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp.Converters
{
    public class CepFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            
            string cep = value.ToString().Replace("-", "");
            
            if (cep.Length <= 5)
                return cep;
            else if (cep.Length <= 8)
                return $"{cep.Substring(0, 5)}-{cep.Substring(5)}";
            
            return cep;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            return value.ToString().Replace("-", "");
        }
    }
}

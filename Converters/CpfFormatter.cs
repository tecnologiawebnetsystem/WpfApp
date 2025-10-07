using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp.Converters
{
    public class CpfFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            
            string cpf = value.ToString().Replace(".", "").Replace("-", "");
            
            if (cpf.Length <= 3)
                return cpf;
            else if (cpf.Length <= 6)
                return $"{cpf.Substring(0, 3)}.{cpf.Substring(3)}";
            else if (cpf.Length <= 9)
                return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6)}";
            else if (cpf.Length <= 11)
                return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9)}";
            
            return cpf;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            return value.ToString().Replace(".", "").Replace("-", "");
        }
    }
}

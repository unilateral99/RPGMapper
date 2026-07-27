using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RPGMapper.Conversores
{
    public class ZoomPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type tagertType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return (double)value * 100 + "%";
            }
            return null;
        }

        public object ConvertBack(object value, Type tagertType, object paramenter, System.Globalization.CultureInfo culture)
        {
            if (double.TryParse(value.ToString(), out double result)) 
            {
                return result / 100.0;
            }

            return Binding.DoNothing;
        }
    }
}

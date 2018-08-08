using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace SharedCode.Converters
{
    public class ColorStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value.ToString() == "Pending")
                    return "#FFA000";
                else if (value.ToString() == "Resolved")
                    return "#2E7D32";
                else if (value.ToString() == "quotation")
                    return "#2b5123";
                else if (value.ToString() == "Ack")
                    return "#d34836";
                else if (value.ToString() == null)
                    return "ion-android-warning";
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

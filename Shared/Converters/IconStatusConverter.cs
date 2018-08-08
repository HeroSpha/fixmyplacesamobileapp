using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace SharedCode.Converters
{
    public class IconStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            try
            {
                if (value.ToString() == "Pending")
                    return "ion-clock";
                else if (value.ToString() == "Resolved")
                    return "ion-android-checkbox-outline";
                else if (value.ToString() == "quotation")
                    return "md-account-balance-wallet";
                else if (value.ToString() == "Ack")
                    return "ion-android-warning";
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
            if (value == null)
                return null;
            return null;
        }
    }
}

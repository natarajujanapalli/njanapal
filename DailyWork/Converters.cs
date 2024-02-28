using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DailyWork
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string state = "";

            if (value != null && !string.IsNullOrEmpty(value.ToString()))
                state = ((string)value).ToUpper();

            switch (state)
            {
                case "1":
                    return "Open";

                case "2":
                    return "In Progress";

                case "3":
                    return "Waiting";

                case "4":
                    return "Completed";

                default:
                    return "";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string state = "";

            if (value != null)
                state = ((string)value).ToUpper();

            switch (state)
            {
                case "Open":
                    return "1";

                case "In Progress":
                    return "2";

                case "Waiting":
                    return "3";

                case "Completed":
                    return "4";

                default:
                    return "";
            }
        }
    }

    public class PriorityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string state = "";

            if (value != null && !string.IsNullOrEmpty(value.ToString()))
                state = ((string)value).ToUpper();

            switch (state)
            {
                case "1":
                    return "High";

                case "2":
                    return "Normal";

                case "3":
                    return "Low";

                default:
                    return "";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string state = "";

            if (value != null)
                state = ((string)value).ToUpper();

            switch (state)
            {
                case "High":
                    return "1";

                case "Normal":
                    return "2";

                case "Low":
                    return "3";

                default:
                    return "";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

using TimePlayer.Entity;


namespace TimePlayer.Converters
{
	public class TimeRangeToStringConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			TimeSpan from = (TimeSpan)values[0];
			TimeSpan to = (TimeSpan)values[1];

			return TimeRange.ToString(from, to);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			string str = (string)value;
			TimeSpan from, to;

			return TimeRange.TryParse(str, out from, out to) ? new object[2] { from, to } : null;
		}
	}
}

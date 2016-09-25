using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace TimePlayer.Entity
{
	public class TimeRange : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;


		private TimeSpan _from, _to;

		public TimeSpan FromTime
		{
			get { return _from; }
			set
			{
				_from = value;
				NotifyPropertyChanged("FromTime");
			}
		}

		public TimeSpan ToTime
		{
			get { return _to; }
			set
			{
				_to = value;
				NotifyPropertyChanged("ToTime");
			}
		}


		public TimeRange(TimeSpan from, TimeSpan to)
		{
			FromTime = from;
			ToTime = to;
		}


		public bool Contains(TimeSpan time)
		{
			if (FromTime <= time && time <= ToTime)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		public TimeRange Clone()
		{
			return new TimeRange(FromTime, ToTime);
		}


		public override string ToString()
		{
			return ToString(FromTime, ToTime);
		}

		private static string ToString(TimeSpan span)
		{
			string minutes = span.Minutes.ToString();
			if (minutes.Length < 2)
			{
				minutes = "0" + minutes;
			}

			string seconds = span.Seconds.ToString();
			if (seconds.Length < 2)
			{
				seconds = "0" + seconds;
			}

			return string.Format("{0}:{1}", minutes, seconds);
		}

		public static string ToString(TimeSpan from, TimeSpan to)
		{
			return string.Format("{0} - {1}", ToString(from), ToString(to));
		}

		private static bool TryParseTimeSpan(string s, out TimeSpan result)
		{
			string[] parts = s.Split(':');
			if (parts.Length == 2)
			{
				int mins = 0, secs = 0;
				if (int.TryParse(parts[0], out mins) && int.TryParse(parts[1], out secs))
				{
					mins = Math.Min(59, mins);
					secs = Math.Min(59, secs);
					result = new TimeSpan(0, mins, secs);
					return true;
				}
			}

			result = new TimeSpan();
			return false;
		}


		public static bool TryParse(string s, out TimeRange result)
		{
			result = new TimeRange(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0));

			string[] parts = s.Split('-');
			if (parts.Length != 2)
			{
				return false;
			}

			TimeSpan from, to;
			if (TryParseTimeSpan(parts[0], out from) && TryParseTimeSpan(parts[1], out to))
			{
				result = new TimeRange(from, to);
				return true;
			}

			return false;
		}


		public static bool TryParse(string s, out TimeSpan from, out TimeSpan to)
		{
			string[] parts = s.Split('-');
			if (parts.Length == 2 && TryParseTimeSpan(parts[0], out from) && TryParseTimeSpan(parts[1], out to))
			{
				return true;
			}
			else
			{
				from = new TimeSpan();
				to = new TimeSpan();
				return false;
			}
		}


		public static bool operator == (TimeRange left, TimeRange right)
		{
			return (left.FromTime == right.FromTime && left.ToTime == right.ToTime);
		}

		public static bool operator !=(TimeRange left, TimeRange right)
		{
			return (left.FromTime != right.FromTime || left.ToTime != right.ToTime);
		}


		private void NotifyPropertyChanged(string property)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}
	}
}

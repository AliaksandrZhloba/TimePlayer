using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TimePlayer.Entity
{
	public class VideoPartInfo
	{
		public string Title
		{
			get;
			set;
		}


		public TimeRange TimeRange
		{
			get;
			set;
		}


		public VideoPartInfo(TimeRange timeRange, string title)
		{
			Title = title;
			TimeRange = timeRange;
		}

		public VideoPartInfo Clone()
		{
			return new VideoPartInfo(TimeRange, Title);
		}


		public bool EqualsTo(VideoPartInfo info)
		{
			return (info.Title == Title && info.TimeRange == TimeRange);
		}
	}
}

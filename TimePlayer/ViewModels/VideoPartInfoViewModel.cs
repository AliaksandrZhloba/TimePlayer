using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TimePlayer.Entity;
using TimePlayer.Commands;


namespace TimePlayer.ViewModels
{
	public class VideoPartInfoViewModel : ViewModelBase
	{
		public VideoPartInfo VideoPartInfo
		{
			get;
			private set;
		}

		public TimeSpan FromTime
		{
			get { return VideoPartInfo.TimeRange.FromTime; }
			set
			{
				VideoPartInfo.TimeRange.FromTime = value;
				NotifyPropertyChanged("FromTime");
			}
		}

		public TimeSpan ToTime
		{
			get { return VideoPartInfo.TimeRange.ToTime; }
			set
			{
				VideoPartInfo.TimeRange.ToTime = value;
				NotifyPropertyChanged("ToTime");
			}
		}

		public string Title
		{
			get { return VideoPartInfo.Title; }
			set
			{
				VideoPartInfo.Title = value;
				NotifyPropertyChanged("Title");
			}
		}


		public JumpToVideoPartCommand JumpToVideoPartCommand
		{
			get;
			private set;
		}


		public DeleteVideoPartCommand DeleteVideoPartCommand
		{
			get;
			private set;
		}


		public ConfirmDeleteVideoPartCommand ConfirmDeleteVideoPartCommand
		{
			get;
			private set;
		}

		private bool _isActive = false;
		private bool _isDragging = false;
		private bool _isReadOnly = false;

		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				NotifyPropertyChanged("IsActive");
			}
		}


		public bool IsDragging
		{
			get { return _isDragging; }
			set
			{
				_isDragging = value;
				NotifyPropertyChanged("IsDragging");
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
			private set
			{
				_isReadOnly = value;
				NotifyPropertyChanged("IsReadOnly");
			}
		}


		public VideoPartInfoViewModel(VideoPartInfo info, Action<VideoPartInfoViewModel> jumpCommandExecuted, Action<VideoPartInfoViewModel> deleteVideoInfoCommand, bool isReadOnly = false)
		{
			IsActive = false;
			IsDragging = false;
			IsReadOnly = isReadOnly;

			VideoPartInfo = info;

			JumpToVideoPartCommand = new JumpToVideoPartCommand(() => jumpCommandExecuted(this));
			DeleteVideoPartCommand = new DeleteVideoPartCommand(() => deleteVideoInfoCommand(this));
		}
	}
}

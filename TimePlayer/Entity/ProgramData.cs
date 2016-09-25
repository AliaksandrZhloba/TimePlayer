using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using TimePlayer.ViewModels;


namespace TimePlayer.Entity
{
	public class ProgramData
	{
		public VPListData VPListData
		{
			get;
			private set;
		}

		public VideoPartInfo CurrentVideoPartInfo
		{
			get { return CurrentVideoPartVideoViewModel.VideoPartInfo; }
		}


		public ObservableCollection<VideoPartInfoViewModel> ViewModel
		{
			get;
			set;
		}

		public VideoPartInfoViewModel CurrentVideoPartVideoViewModel
		{
			get;
			set;
		}

		public VideoPartInfoViewModel FullVideoPartViewModel
		{
			get;
			set;
		}

		public VideoPartInfo FullVideoPartInfo
		{
			get { return FullVideoPartViewModel.VideoPartInfo; }
		}

		public ObservableCollection<VideoPartInfo> InfosCopy;
		public ObservableCollection<VideoPartInfoViewModel> InfoModelsCopy;


		public ProgramData(VPListData vpListData)
		{
			VPListData = vpListData;
			FullVideoPartViewModel = null;
			CurrentVideoPartVideoViewModel = null;
		}
	}
}

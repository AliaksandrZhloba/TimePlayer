using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


namespace TimePlayer.Entity
{
	public class VPListData
	{
		private ObservableCollection<VideoPartInfo> _src;

		public string Path
		{
			get;
			private set;
		}

		public string VideoFile
		{
			get;
			private set;
		}

		public ObservableCollection<VideoPartInfo> VideoPartInfos
		{
			get;
			set;
		}


		public VPListData(string path, string videoFile, ObservableCollection<VideoPartInfo> infos)
		{
			Path = path;
			VideoFile = videoFile;
			VideoPartInfos = infos;

			_src = new ObservableCollection<VideoPartInfo>();
			foreach (VideoPartInfo info in infos)
			{
				_src.Add(info.Clone());
			}
		}


		public VPListData(string path, string videoFile)
		{
			Path = path;
			VideoFile = videoFile;
			VideoPartInfos = new ObservableCollection<VideoPartInfo>();
			_src = new ObservableCollection<VideoPartInfo>();
		}


		public void InsertFullInfo(VideoPartInfo fullInfo)
		{
			VideoPartInfos.Insert(0, fullInfo);
			_src.Insert(0, fullInfo.Clone());
		}

		public void RemoveFullInfo()
		{
			VideoPartInfos.RemoveAt(0);
			_src.RemoveAt(0);
		}


		public bool IsChanged()
		{
			if (_src.Count != VideoPartInfos.Count)
			{
				return true;
			}

			for (int i = 0; i < _src.Count; i++)
			{
				if (!_src[i].EqualsTo(VideoPartInfos[i]))
				{
					return true;
				}
			}

			return false;
		}
	}
}

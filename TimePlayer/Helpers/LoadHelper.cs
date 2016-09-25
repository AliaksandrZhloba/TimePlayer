using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using TimePlayer.Entity;


namespace TimePlayer.Helpers
{
	public static class LoadHelper
	{
		public static VPListData LoadVPListData(string path)
		{
			ObservableCollection<VideoPartInfo> infos = new ObservableCollection<VideoPartInfo>();

			string file = null;
			string title = null, s_range = null;
			TimeRange range;

			using (XmlTextReader reader = new XmlTextReader(path))
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if ((string.Compare(reader.Name, "VideoParts", true) == 0))
						{
							while (reader.MoveToNextAttribute())
							{
								if (string.Compare(reader.Name, "File", true) == 0)
								{
									file = reader.Value;
								}
							}
						}
						else if ((string.Compare(reader.Name, "VideoPart", true) == 0))
						{
							while (reader.MoveToNextAttribute())
							{
								if (string.Compare(reader.Name, "Title", true) == 0)
								{
									title = reader.Value;
								}
								else if (string.Compare(reader.Name, "TimeRange", true) == 0)
								{
									s_range = reader.Value;
								}
							}

							if (TimeRange.TryParse(s_range, out range))
							{
								infos.Add(new VideoPartInfo(range, title));
							}
						}
					}
				}
			}

			return new VPListData(path, file, infos);
		}


		public static void SaveVPListData(VPListData data)
		{
			if (File.Exists(data.Path))
			{
				string bakFilePath = data.Path + ".bak";
				try
				{
					if (File.Exists(bakFilePath))
					{
						File.Delete(bakFilePath);
					}
					File.Move(data.Path, bakFilePath);
				}
				catch (Exception)
				{ }
			}

			using (XmlTextWriter writer = new XmlTextWriter(data.Path, Encoding.Unicode))
			{
				writer.WriteStartDocument();
				writer.WriteWhitespace(Environment.NewLine);
				writer.WriteStartElement("VideoParts");
				writer.WriteAttributeString("File", data.VideoFile);
				writer.WriteWhitespace(Environment.NewLine);

				foreach (VideoPartInfo info in data.VideoPartInfos)
				{
					writer.WriteStartElement("VideoPart");
					writer.WriteAttributeString("Title", info.Title);
					writer.WriteAttributeString("TimeRange", info.TimeRange.ToString());

					writer.WriteEndElement();
					writer.WriteWhitespace(Environment.NewLine);
				}

				writer.WriteEndDocument();
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using TimePlayer.Entity;
using TimePlayer.Helpers;
using TimePlayer.ViewModels;


namespace TimePlayer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string PARTSDATAFILE_EXTENSION = ".vplist";
		private const double VOLUME_STEP = 0.1;

		private ProgramData _data;


		public bool IsDesignMode
		{
			get;
			private set;
		}


		public MainWindow()
		{
			InitializeComponent();
			IsDesignMode = false;

			mePlayer.PositionChanged = (TimeSpan position) =>
				{
					if (position >= _data.CurrentVideoPartInfo.TimeRange.ToTime)
					{
						mePlayer.Pause();
						mePlayer.Position = _data.CurrentVideoPartInfo.TimeRange.FromTime;
					}
				};

			timeSlider.JumpedTo = (TimeSpan value) =>
				{
					if (!_data.CurrentVideoPartInfo.TimeRange.Contains(value))
					{
						ActiveteNewVideoPart(_data.FullVideoPartViewModel);
					}
				};

			videoPartsList.MovePart = (VideoPartInfoViewModel viewModel, int newIndex) =>
				{
					_data.VPListData.VideoPartInfos.Remove(viewModel.VideoPartInfo);
					_data.VPListData.VideoPartInfos.Insert(newIndex, viewModel.VideoPartInfo);

					_data.ViewModel.Remove(viewModel);
					_data.ViewModel.Insert(newIndex, viewModel);
				};

			string[] args = Environment.GetCommandLineArgs();
			if (args.Length == 2)
			{
				string file = args[1];
				OpenFile(file);
			}
		}



		private void MainWindow_Closing(object sender, CancelEventArgs e)
		{
			SaveVideoPartsInfo();
		}


		private void MainWindow_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("FileDrop"))
			{
				string[] files = (string[])e.Data.GetData("FileDrop");
				if (files != null && files.Length == 1)
				{
					string file = files[0];
					OpenFile(file);
				}
			}
		}


		public void OpenFile(string file)
		{
			SaveVideoPartsInfo();

			string mediaFile = null, partsDataFile = null;
			string fname = System.IO.Path.GetFileNameWithoutExtension(file);
			string extension = System.IO.Path.GetExtension(file);
			string directory = System.IO.Path.GetDirectoryName(file);

			Title = "TimePlayer: " + file;

			if (extension == PARTSDATAFILE_EXTENSION)
			{
				partsDataFile = file;
			}
			else
			{
				mediaFile = file;
				partsDataFile = System.IO.Path.Combine(directory, string.Format("{0}{1}", fname, PARTSDATAFILE_EXTENSION));
			}

			mePlayer.Stop();

			_data = null;
			try
			{
				_data = new ProgramData(LoadHelper.LoadVPListData(partsDataFile));
			}
			catch (Exception)
			{
				if (mediaFile == null)
				{
					return;
				}
				_data = new ProgramData(new VPListData(partsDataFile, System.IO.Path.GetFileName(mediaFile)));

				_data.ViewModel = null;
				videoPartsList.ItemsSource = null;
				timeSlider.IsEnabled = false;
			}

			if (_data != null)
			{
				if (mediaFile == null)
				{
					mediaFile = System.IO.Path.Combine(directory, _data.VPListData.VideoFile);
				}
				mePlayer.OpenFile(mediaFile, () =>
				{
					ObservableCollection<VideoPartInfoViewModel> vmodel = new ObservableCollection<VideoPartInfoViewModel>();
					foreach (VideoPartInfo info in _data.VPListData.VideoPartInfos)
					{
						vmodel.Add(new VideoPartInfoViewModel(info, JumpCommandExecuted, DeleteCommandExecuted));
					}

					VideoPartInfo fullInfo = new VideoPartInfo(new TimeRange(TimeSpan.FromSeconds(0), mePlayer.Duration), "Full");
					VideoPartInfoViewModel fullVideModel = new VideoPartInfoViewModel(fullInfo, JumpCommandExecuted, DeleteCommandExecuted, isReadOnly: true);

					_data.VPListData.InsertFullInfo(fullInfo);
					vmodel.Insert(0, fullVideModel);


					_data.FullVideoPartViewModel = vmodel[0];

					_data.ViewModel = vmodel;
					videoPartsList.ItemsSource = vmodel;

					timeSlider.SetFullTimeRange(mePlayer.Duration);
					timeSlider.IsEnabled = true;

					JumpCommandExecuted(vmodel[0]);
				});
			}
		}

		private void SaveVideoPartsInfo()
		{
			if (_data != null && _data.VPListData.IsChanged())
			{
				_data.VPListData.RemoveFullInfo();
				Helpers.LoadHelper.SaveVPListData(_data.VPListData);
			}
		}

		private void DeleteCommandExecuted(VideoPartInfoViewModel info)
		{
			if (_data.CurrentVideoPartVideoViewModel == info)
			{
				JumpCommandExecuted(_data.FullVideoPartViewModel);
			}

			videoPartsList.ItemsSource.Remove(info);
			_data.VPListData.VideoPartInfos.Remove(info.VideoPartInfo);
		}

		private void JumpCommandExecuted(VideoPartInfoViewModel infoViewModel)
		{
			if (_data.CurrentVideoPartVideoViewModel != infoViewModel)
			{
				ActiveteNewVideoPart(infoViewModel);
				mePlayer.Position = infoViewModel.VideoPartInfo.TimeRange.FromTime;
			}
			else
			{
				if (!IsDesignMode && !mePlayer.IsPlaying)
				{
					mePlayer.Position = infoViewModel.VideoPartInfo.TimeRange.FromTime;
					mePlayer.Play();
				}
			}
		}

		private void ActiveteNewVideoPart(VideoPartInfoViewModel infoViewModel)
		{
			if (_data.CurrentVideoPartVideoViewModel!= null)
			{
				_data.CurrentVideoPartVideoViewModel.IsActive = false;
			}

			_data.CurrentVideoPartVideoViewModel = infoViewModel;
			infoViewModel.IsActive = true;

			timeSlider.DataContext = infoViewModel;
			timeSlider.Value = infoViewModel.FromTime;
		}


		private void btnVideoControl_StartedPlaying(object sender, RoutedEventArgs e)
		{
			mePlayer.Play();
		}

		private void btnVideoControl_PausedPlaying(object sender, RoutedEventArgs e)
		{
			mePlayer.Pause();
		}


		private void mePlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!IsDesignMode && mePlayer.IsReady)
			{
				ToggleVideoPlayer();
			}
		}


		private void MainWindow_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			volumeControl.Volume += (e.Delta > 0) ? VOLUME_STEP : -VOLUME_STEP;
		}

		private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (!IsDesignMode)
			{
				if (e.Key == Key.Up)
				{
					volumeControl.Volume += VOLUME_STEP;
					e.Handled = true;
				}
				else if (e.Key == Key.Down)
				{
					volumeControl.Volume -= VOLUME_STEP;
					e.Handled = true;
				}
				else
				{
					if (mePlayer.IsReady)
					{
						if (e.Key == Key.Space || e.Key == Key.Enter)
						{
							ToggleVideoPlayer();
							e.Handled = true;
						}
						else if (e.Key == Key.Left)
						{
							mePlayer.MoveBack(2);
							e.Handled = true;
						}
						else if (e.Key == Key.Right)
						{
							mePlayer.MoveForward(2);
							e.Handled = true;
						}
					}
				}
			}
		}

		private void btnVideoControl_Clicked(object sender, RoutedEventArgs e)
		{
			ToggleVideoPlayer();
		}

		private void ToggleVideoPlayer()
		{
			if (mePlayer.IsPlaying)
			{
				mePlayer.Pause();
			}
			else
			{
				mePlayer.Play();
			}
		}


		private void btnEditVideoPartsList_Click(object sender, RoutedEventArgs e)
		{
			if (mePlayer.IsPlaying)
			{
				mePlayer.Pause();
			}

			_data.InfosCopy = new ObservableCollection<VideoPartInfo>();
			_data.InfoModelsCopy = new ObservableCollection<VideoPartInfoViewModel>();

			VideoPartInfo fullCopy = _data.VPListData.VideoPartInfos[0].Clone();
			_data.InfosCopy.Add(fullCopy);
			_data.InfoModelsCopy.Add(new VideoPartInfoViewModel(fullCopy, JumpCommandExecuted, DeleteCommandExecuted, isReadOnly: true));

			for (int i = 1; i < _data.VPListData.VideoPartInfos.Count; i++)
			{
				VideoPartInfo copy = _data.VPListData.VideoPartInfos[i].Clone();
				_data.InfosCopy.Add(copy);
				_data.InfoModelsCopy.Add(new VideoPartInfoViewModel(copy, JumpCommandExecuted, DeleteCommandExecuted));
			}

			SetDesignMode(true);
		}

		private void btnApplyChanges_Click(object sender, RoutedEventArgs e)
		{
			SetDesignMode(false);

			if (_data.CurrentVideoPartInfo.TimeRange.FromTime > timeSlider.Value)
			{
				timeSlider.Value = _data.CurrentVideoPartInfo.TimeRange.FromTime;
			}
			else if (_data.CurrentVideoPartInfo.TimeRange.ToTime < timeSlider.Value)
			{
				timeSlider.Value = _data.CurrentVideoPartInfo.TimeRange.ToTime;
			}
		}

		private void btnCancelChanges_Click(object sender, RoutedEventArgs e)
		{
			SetDesignMode(false);

			_data.VPListData.VideoPartInfos = _data.InfosCopy;
			videoPartsList.ItemsSource = _data.InfoModelsCopy;
			_data.FullVideoPartViewModel = _data.InfoModelsCopy[0];
			JumpCommandExecuted(_data.FullVideoPartViewModel);

			_data.InfosCopy = null;
			_data.InfoModelsCopy = null;
		}


		private void SetDesignMode(bool designMode)
		{
			IsDesignMode = videoPartsList.DesignMode = timeSlider.DesignMode = designMode;
			btnVideoControl.IsEnabled = AllowDrop = !designMode;
		}


		private void btnAddVideoPart_Click(object sender, RoutedEventArgs e)
		{
			VideoPartInfo newPartInfo = new VideoPartInfo(_data.FullVideoPartInfo.TimeRange.Clone(), string.Format("ex.{0}", _data.VPListData.VideoPartInfos.Count));
			VideoPartInfoViewModel model = new VideoPartInfoViewModel(newPartInfo, JumpCommandExecuted, DeleteCommandExecuted);

			_data.VPListData.VideoPartInfos.Add(newPartInfo);
			videoPartsList.ItemsSource.Add(model);
			videoPartsList.ScrollToRightEnd();

			ActiveteNewVideoPart(model);
		}
	}
}

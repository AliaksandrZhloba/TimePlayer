using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace TimePlayer.UserControls
{
	/// <summary>
	/// Interaction logic for VideoPlayer.xaml
	/// </summary>
	public partial class VideoPlayer : UserControl, INotifyPropertyChanged, IDisposable
	{
		public TimeSpan Position
		{
			get { return mePlayer.Position; }
			set { mePlayer.Position = value; }
		}


		public event PropertyChangedEventHandler PropertyChanged;

		public Action<TimeSpan> PositionChanged;

		private Action _fileOpened;
		private CancellationTokenSource _cts;
		private bool _isReady, _isPlaying;



		public TimeSpan Duration
		{
			get;
			private set;
		}

		public double Volume
		{
			get { return mePlayer.Volume; }
			set { mePlayer.Volume = value; }
		}


		public bool IsReady
		{
			get { return _isReady; }
			private set
			{
				_isReady = value;
				NotifyPropertyChanged("IsReady");
			}
		}

		public bool IsPlaying
		{
			get { return _isPlaying; }
			private set
			{
				_isPlaying = value;
				NotifyPropertyChanged("IsPlaying");
			}
		}

		public VideoPlayer()
		{
			InitializeComponent();

			_isReady = false;
			_isPlaying = false;
		}

		public void Dispose()
		{
			if (_cts != null)
			{
				_cts.Cancel();
			}
		}


		public void OpenFile(string path, Action fileOpened)
		{
			_fileOpened = fileOpened;

			mePlayer.Source = new Uri(path);
			mePlayer.Play();
		}

		public void Play()
		{
			mePlayer.Play();

			IsPlaying = true;
		}

		public void Pause()
		{
			mePlayer.Pause();

			IsPlaying = false;
		}

		public void Stop()
		{
			mePlayer.Stop();
			mePlayer.Source = null;

			IsReady = false;
			IsPlaying = false;
		}


		private void mePlayer_MediaOpened(object sender, RoutedEventArgs e)
		{
			Duration = mePlayer.NaturalDuration.TimeSpan;
			_fileOpened();

			IsReady = true;
			IsPlaying = true;

			if (_cts == null)
			{
				_cts = new CancellationTokenSource();
				Task.Factory.StartNew(() =>
				{
					while (!_cts.IsCancellationRequested)
					{
						//if (IsPlaying)
						{
							Dispatcher.Invoke((Action)(() =>
							{
								NotifyPropertyChanged("Position");
								PositionChanged(Position);
							}));
						}

						System.Threading.Thread.Sleep(50);
					}
				});
			}
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using TimePlayer.ViewModels;


namespace TimePlayer.UserControls
{
	/// <summary>
	/// Interaction logic for VideoPartsList.xaml
	/// </summary>
	public partial class VideoPartsList : UserControl
	{
		public static readonly DependencyProperty
			ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<VideoPartInfoViewModel>), typeof(VideoPartsList)),
			DesignModePorperty = DependencyProperty.Register("DesignMode", typeof(bool), typeof(VideoPartsList));


		public ObservableCollection<VideoPartInfoViewModel> ItemsSource
		{
			get { return (ObservableCollection<VideoPartInfoViewModel>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public bool DesignMode
		{
			get { return (bool)GetValue(DesignModePorperty); }
			set { SetValue(DesignModePorperty, value); }
		}

		public Action<VideoPartInfoViewModel, int> MovePart;


		public VideoPartsList()
		{
			InitializeComponent();
		}


		public void ScrollToRightEnd()
		{
			scroll.ScrollToRightEnd();
		}


		private void ScrollUp(object sender, RoutedEventArgs e)
		{
			scroll.LineLeft();
		}

		private void ScrollDown(object sender, RoutedEventArgs e)
		{
			scroll.LineRight();
		}


		private void imgMove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Image img = (Image)sender;
			Button btn = Helpers.VisualParentHelper<Button>.GetVisualParent(img);
			VideoPartInfoViewModel info = (VideoPartInfoViewModel)btn.DataContext;

			info.IsDragging = true;
			DragDrop.DoDragDrop(btn, info, DragDropEffects.Move);
			info.IsDragging = false;
		}


		private void spVideoParts_PreviewDrop(object sender, DragEventArgs e)
		{
			VideoPartInfoViewModel partInfo = e.Data.GetData(typeof(VideoPartInfoViewModel)) as VideoPartInfoViewModel;
			if (partInfo != null)
			{
				Point mp = e.GetPosition(icVideoParts);
				Vector vect = new Vector(mp.X, mp.Y);

				double min = double.MaxValue;
				int newIndex = -1;
				UIElement element = null;

				for (int i = 0; i < ItemsSource.Count; i++)
				{
					UIElement uiElement = (UIElement)icVideoParts.ItemContainerGenerator.ContainerFromIndex(i);
					Vector offset = VisualTreeHelper.GetOffset(uiElement);

					Vector delta = vect - offset;
					if (delta.Length < min)
					{
						min = delta.Length;
						newIndex = i;
						element = uiElement;
					}
				}

				if (newIndex == 0)
				{
					newIndex = 1;
				}

				int oldIndex = ItemsSource.IndexOf(partInfo);
				if (newIndex != oldIndex)
				{
					MovePart(partInfo, newIndex);
				}
			}
		}


		private void spVideoParts_PreviewDragOver(object sender, DragEventArgs e)
		{
			/*VideoPartInfoViewModel partInfo = e.Data.GetData(typeof(VideoPartInfoViewModel)) as VideoPartInfoViewModel;
			if (partInfo != null)
			{
				Point mp = e.GetPosition(icVideoParts);
				Vector vect = new Vector(mp.X, mp.Y);

				double min = double.MaxValue;
				int index = -1;
				UIElement element = null;

				for (int i = 0; i < ItemsSource.Count; i++)
				{
					UIElement uiElement = (UIElement)icVideoParts.ItemContainerGenerator.ContainerFromIndex(i);
					Vector offset = VisualTreeHelper.GetOffset(uiElement);

					Vector delta = vect - offset;
					if (delta.Length < min)
					{
						min = delta.Length;
						index = i;
						element = uiElement;
					}
				}

				System.Diagnostics.Debug.WriteLine(index);
			}*/
		}


		private void btnJumpToVideoPart_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			TextBox tbox = Keyboard.FocusedElement as TextBox;
			if (tbox != null)
			{
				var scope = FocusManager.GetFocusScope(tbox);		// elem is the UIElement to unfocus
				FocusManager.SetFocusedElement(scope, null);			// remove logical focus
				Keyboard.ClearFocus();							// remove keyboard focus
			}

			DependencyObject d = (DependencyObject)sender;
			Button btn = Helpers.VisualParentHelper<Button>.GetVisualParent(d);

			VideoPartInfoViewModel info = (VideoPartInfoViewModel)btn.DataContext;
			info.JumpToVideoPartCommand.Execute(null);
		}
	}
}

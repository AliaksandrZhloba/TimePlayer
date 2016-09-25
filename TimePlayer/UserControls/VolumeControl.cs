using System;
using System.Collections.Generic;
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

namespace TimePlayer.UserControls
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:TimePlayer.UserControls"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:TimePlayer.UserControls;assembly=TimePlayer.UserControls"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <MyNamespace:VolumeControl/>
	///
	/// </summary>
	public class VolumeControl : Control
	{
		public static readonly DependencyProperty
			VolumeProperty = DependencyProperty.Register("Volume", typeof(double), typeof(VolumeControl), new PropertyMetadata(0.8, VolumePropertyChanged));

		public double Volume
		{
			get { return (double)GetValue(VolumeProperty); }
			set { SetValue(VolumeProperty, value); }
		}

		private static bool ValidateVolumeValue(object value)
		{
			double volume = (double)value;
			return (volume >= 0.0 && volume <= 1.0);
		}

		private static void VolumePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			VolumeControl vcontrol = (VolumeControl)d;
			if (vcontrol.Volume > 1.0)
			{
				vcontrol.Volume = 1.0;
			}
			else if (vcontrol.Volume < 0.0)
			{
				vcontrol.Volume = 0.0;
			}
			vcontrol.RaiseEvent(new RoutedEventArgs(VolumeChangedEvent));
		}


		public static readonly RoutedEvent
			VolumeChangedEvent = EventManager.RegisterRoutedEvent("VolumeChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VolumeControl));


		public event RoutedEventHandler VolumeChanged
		{
			add { AddHandler(VolumeChangedEvent, value); }
			remove { RemoveHandler(VolumeChangedEvent, value); }
		}


		static VolumeControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(VolumeControl), new FrameworkPropertyMetadata(typeof(VolumeControl)));
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	///     <MyNamespace:TimeSlider/>
	///
	/// </summary>
	public class TimeSlider : Control
	{
		private Duration _animationDuration = new Duration(TimeSpan.FromSeconds(0.2));
		private Converters.TimeSpanToDoubleConverter _timeSpanToDoubleConverter = new Converters.TimeSpanToDoubleConverter();

		private bool _valueShifting = false, _rangeStartShifting = false, _rangeEndShifting = false;
		private Slider valueSlider;
		private Slider rangeStartSlider;
		private Slider rangeEndSlider;

		public Action<TimeSpan> JumpedTo;


		public static readonly DependencyProperty
			ValueProperty = DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimeSlider), new PropertyMetadata(ValuePropertyChanged)),
			MinimumProperty = DependencyProperty.Register("Minimum", typeof(TimeSpan), typeof(TimeSlider), new PropertyMetadata(TimeSpan.FromSeconds(0))),
			MaximumProperty = DependencyProperty.Register("Maximum", typeof(TimeSpan), typeof(TimeSlider), new PropertyMetadata(TimeSpan.FromSeconds(1))),
			RangeStartProperty = DependencyProperty.Register("RangeStart", typeof(TimeSpan), typeof(TimeSlider), new PropertyMetadata(RangeStartPropertyChanged)),
			RangeEndProperty = DependencyProperty.Register("RangeEnd", typeof(TimeSpan), typeof(TimeSlider), new PropertyMetadata(RangeEndPropertyChanged)),
			DesignModePorperty = DependencyProperty.Register("DesignMode", typeof(bool), typeof(TimeSlider));

		private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TimeSlider slider = (TimeSlider)d;
			slider.UpdateSliderPosition();
		}

		private void UpdateSliderPosition()
		{
			if (!_valueShifting)
			{
				valueSlider.ValueChanged -= valueSlider_ValueChanged;
				valueSlider.Value = Value.TotalSeconds;
				valueSlider.ValueChanged += valueSlider_ValueChanged;
			}
		}

		public TimeSpan Value
		{
			get { return (TimeSpan)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}


		private static void RangeStartPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TimeSlider slider = (TimeSlider)d;
			if (!slider._rangeStartShifting)
			{
				slider.rangeStartSlider.Value = slider.RangeStart.TotalSeconds;
				/*DoubleAnimation daRangeStart = new DoubleAnimation(slider.RangeStart.TotalSeconds, slider._animationDuration);
				slider.rangeStartSlider.BeginAnimation(Slider.ValueProperty, daRangeStart);*/
			}
		}

		private static void RangeEndPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TimeSlider slider = (TimeSlider)d;
			if (!slider._rangeEndShifting)
			{
				slider.rangeEndSlider.Value = slider.RangeEnd.TotalSeconds;
				/*DoubleAnimation daRangeEnd = new DoubleAnimation(slider.RangeEnd.TotalSeconds, slider._animationDuration);
				slider.rangeEndSlider.BeginAnimation(Slider.ValueProperty, daRangeEnd);*/
			}
		}

		public TimeSpan Minimum
		{
			get { return (TimeSpan)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}

		public TimeSpan Maximum
		{
			get { return (TimeSpan)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}

		public TimeSpan RangeStart
		{
			get { return (TimeSpan)GetValue(RangeStartProperty); }
			set { SetValue(RangeStartProperty, value); }
		}

		public TimeSpan RangeEnd
		{
			get { return (TimeSpan)GetValue(RangeEndProperty); }
			set { SetValue(RangeEndProperty, value); }
		}

		public bool DesignMode
		{
			get { return (bool)GetValue(DesignModePorperty); }
			set { SetValue(DesignModePorperty, value); }
		}



		static TimeSlider()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSlider), new FrameworkPropertyMetadata(typeof(TimeSlider)));
		}

		public TimeSlider()
		{
			DataContextChanged += TimeSlider_DataContextChanged;
		}

		private void TimeSlider_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			rangeEndSlider.ValueChanged -= rangeEndSlider_ValueChanged;
			rangeStartSlider.ValueChanged -= rangeStartSlider_ValueChanged;

			SetBinding(TimeSlider.RangeStartProperty, new Binding("FromTime") { Source = DataContext, Mode = BindingMode.TwoWay });
			SetBinding(TimeSlider.RangeEndProperty, new Binding("ToTime") { Source = DataContext, Mode = BindingMode.TwoWay });

			rangeEndSlider.ValueChanged += rangeEndSlider_ValueChanged;
			rangeStartSlider.ValueChanged += rangeStartSlider_ValueChanged;
		}


		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			valueSlider = (Slider)Template.FindName("PART_ValueSlider", this);
			valueSlider.GotMouseCapture += valueSlider_GotMouseCapture;
			valueSlider.LostMouseCapture += valueSlider_LostMouseCapture;
			valueSlider.ValueChanged += valueSlider_ValueChanged;

			rangeStartSlider = (Slider)Template.FindName("PART_RangeStartSlider", this);
			rangeStartSlider.GotMouseCapture += rangeStartSlider_GotMouseCapture;
			rangeStartSlider.LostMouseCapture += rangeStartSlider_LostMouseCapture;
			rangeStartSlider.ValueChanged += rangeStartSlider_ValueChanged;

			rangeEndSlider = (Slider)Template.FindName("PART_RangeEndSlider", this);
			rangeEndSlider.GotMouseCapture += rangeEndSlider_GotMouseCapture;
			rangeEndSlider.LostMouseCapture += rangeEndSlider_LostMouseCapture;
			rangeEndSlider.ValueChanged += rangeEndSlider_ValueChanged;

			valueSlider.ApplyTemplate();
			Rectangle selectionRange = (Rectangle)valueSlider.Template.FindName("PART_SelectionRange", valueSlider);
			selectionRange.Stroke = null;
		}


		private void rangeStartSlider_GotMouseCapture(object sender, MouseEventArgs e)
		{
			_rangeStartShifting = true;
		}

		private void rangeStartSlider_LostMouseCapture(object sender, MouseEventArgs e)
		{
			RangeStart = TimeSpan.FromSeconds(rangeStartSlider.Value);
			_rangeStartShifting = false;
		}

		private void rangeStartSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (e.NewValue > rangeEndSlider.Value)
			{
				rangeStartSlider.ValueChanged -= rangeStartSlider_ValueChanged;
				rangeStartSlider.Value = RangeEnd.TotalSeconds;
				rangeStartSlider.ValueChanged += rangeStartSlider_ValueChanged;

				e.Handled = true;
			}
		}

		private void rangeEndSlider_GotMouseCapture(object sender, MouseEventArgs e)
		{
			_rangeEndShifting = true;
		}

		private void rangeEndSlider_LostMouseCapture(object sender, MouseEventArgs e)
		{
			RangeEnd = TimeSpan.FromSeconds(rangeEndSlider.Value);
			_rangeEndShifting = false;
		}

		private void rangeEndSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (e.NewValue < rangeStartSlider.Value)
			{
				rangeEndSlider.ValueChanged -= rangeEndSlider_ValueChanged;
				rangeEndSlider.Value = RangeStart.TotalSeconds;
				rangeEndSlider.ValueChanged += rangeEndSlider_ValueChanged;

				e.Handled = true;
			}
		}


		private void valueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!_valueShifting)
			{
				JumpedTo(TimeSpan.FromSeconds(e.NewValue));
				Value = TimeSpan.FromSeconds(e.NewValue);
			}
		}

		private void valueSlider_GotMouseCapture(object sender, MouseEventArgs e)
		{
			_valueShifting = true;
		}

		private void valueSlider_LostMouseCapture(object sender, MouseEventArgs e)
		{
			JumpedTo(TimeSpan.FromSeconds(valueSlider.Value));
			Value = TimeSpan.FromSeconds(valueSlider.Value);
			_valueShifting = false;
		}


		public void SetFullTimeRange(TimeSpan range)
		{
			Maximum = range;
			//valueSlider.Maximum = range.TotalSeconds;
		}
	}
}

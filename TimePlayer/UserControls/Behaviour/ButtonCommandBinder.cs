using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace TimePlayer.UserControls.Behaviour
{
	public static class ButtonCommandBinder
	{
		private static readonly DependencyProperty
			PreviewMouseLeftButtonDownCommandProperty = DependencyProperty.RegisterAttached("PreviewMouseLeftButtonDownCommand", typeof(ICommand), typeof(ButtonCommandBinder), new PropertyMetadata(null, OnPreviewMouseLeftButtonDownChanged)),
			MouseDoubleClickCommandProperty = DependencyProperty.RegisterAttached("MouseDoubleClickCommand", typeof(ICommand), typeof(ButtonCommandBinder), new PropertyMetadata(null, OnMouseDoubleClickChanged));


		public static ICommand GetPreviewMouseLeftButtonDownCommand(Button btn)
		{
			return btn.GetValue(PreviewMouseLeftButtonDownCommandProperty) as ICommand;
		}

		public static void SetPreviewMouseLeftButtonDownCommand(Button btn, ICommand cmd)
		{
			btn.SetValue(PreviewMouseLeftButtonDownCommandProperty, cmd);
		}


		public static ICommand GetMouseDoubleClickCommand(Button btn)
		{
			return btn.GetValue(MouseDoubleClickCommandProperty) as ICommand;
		}

		public static void SetMouseDoubleClickCommand(Button btn, ICommand cmd)
		{
			btn.SetValue(MouseDoubleClickCommandProperty, cmd);
		}


		private static void OnPreviewMouseLeftButtonDownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Button btn = (Button)d;

			if (e.OldValue != null)
			{
				btn.PreviewMouseLeftButtonDown -= btn_PreviewMouseLeftButtonDown;
			}

			if (e.NewValue != null)
			{
				btn.PreviewMouseLeftButtonDown += btn_PreviewMouseLeftButtonDown;
			}
		}

		private static void btn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Button btn = (Button)sender;
			ICommand command = GetPreviewMouseLeftButtonDownCommand(btn);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		}


		private static void OnMouseDoubleClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Button btn = (Button)d;

			if (e.OldValue != null)
			{
				btn.MouseDoubleClick -= btn_MouseDoubleClick;
			}

			if (e.NewValue != null)
			{
				btn.MouseDoubleClick += btn_MouseDoubleClick;
			}
		}

		private static void btn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Button btn = (Button)sender;
			ICommand command = GetMouseDoubleClickCommand(btn);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		}
	}
}

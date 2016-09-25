using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;


namespace TimePlayer.Helpers
{
	public static class VisualParentHelper<T> where T : DependencyObject
	{
		public static T GetVisualParent(DependencyObject element) 
		{
			while (element != null && !(element is T))
				element = VisualTreeHelper.GetParent(element);

			return (T)element;
		}
	}
}

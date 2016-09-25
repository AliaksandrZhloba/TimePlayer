using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Microsoft.Shell;


namespace TimePlayer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application, ISingleInstanceApp
	{
		private const string Unique = "Video Time Player Processor App";

		private static App _app;


		[STAThread]
		public static void Main()
		{
			if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
			{
				_app = new App();

				_app.InitializeComponent();
				_app.Run();

				// Allow single instance code to perform cleanup operations
				SingleInstance<App>.Cleanup();
			}
		}


		public bool SignalExternalCommandLineArgs(IList<string> args)
		{
			// handle command line arguments of second instance
			if (args.Count == 2)
			{
				string file = args[1];
				MainWindow window = (MainWindow)_app.MainWindow;
				if (!window.IsDesignMode)
				{
					window.OpenFile(file);
				}
			}

			_app.MainWindow.Activate();

			return true;
		}
	}
}

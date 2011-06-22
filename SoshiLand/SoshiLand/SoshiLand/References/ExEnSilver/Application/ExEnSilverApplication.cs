using System;
using System.Windows;

namespace ExEnSilver
{
	public class ExEnSilverApplication : Application
	{
		public ExEnSilverApplication()
		{
			this.UnhandledException += this.Application_UnhandledException;

			this.Startup += (sender, e) =>
			{
				var mainPage = new MainPage();
				this.RootVisual = mainPage;
				this.SetupMainPage(mainPage);
			};
		}

		protected virtual void SetupMainPage(MainPage page) { }


		#region Exception Handling

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// If the app is running outside of the debugger then report the exception using
			// the browser's exception mechanism. On IE this will display it a yellow alert 
			// icon in the status bar and Firefox will display a script error.
			if(!System.Diagnostics.Debugger.IsAttached)
			{

				// NOTE: This will allow the application to continue running after an exception has been thrown
				// but not handled. 
				// For production applications this error handling should be replaced with something that will 
				// report the error to the website and stop the application.
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
			}
		}
		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch(Exception)
			{
			}
		}

		#endregion
	}
}

using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;

namespace ExEnSilver
{
	public partial class MainPage : Grid
	{
		public delegate Game CreateGameDelegate();

		public MainPage()
		{
			InitializeComponent();


			// TODO: the following code is super-stale:
			
			// Settings for Testing
			// TODO: these should really be configurable as part of the Game class
			var settings = Application.Current.Host.Settings;
			if(settings.MaxFrameRate >= 900) // From HTML
			{
				// game.IsFixedTimeStep = false;
			}
			else
			{
				settings.MaxFrameRate = 60; // Better for GameLoop
			}

		}
	}
}

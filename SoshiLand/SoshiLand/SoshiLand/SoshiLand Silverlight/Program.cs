using System;
using Microsoft.Xna.Framework;

namespace BlankGame
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			using(BlankGameGame game = new BlankGameGame())
			{
				game.Run();
			}
		}
	}
}


using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using Media = System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using System.Collections.Generic;
using System.Diagnostics;
using ExEnSilver.Renderer;

namespace Microsoft.Xna.Framework.Graphics
{
	public class GraphicsDevice
	{
		#region Construction

		internal GraphicsDevice(Game game)
		{
			Root = game;

			this.PresentationParameters = new PresentationParameters();
		}

		internal void UpdatePresentationParameters()
		{
			PresentationParameters.BackBufferWidth = Viewport.Width;
			PresentationParameters.BackBufferHeight = Viewport.Height;
			PresentationParameters.IsFullScreen = false;
			PresentationParameters.DisplayOrientation = DisplayOrientation.Default;
		}

		#endregion


		#region XNA API

		public PresentationParameters PresentationParameters { get; private set; }

		// NOTE: Set is public in the real XNA - it is not currently supported on Silverlight
		public Viewport Viewport { get; internal set; }

		
		private ClearSprite clearSprite;

		/// <summary>
		/// Note that a software surface for clearing is available.
		/// But you'll have to modify this Clear function to use it :)
		/// </summary>
		public void Clear(Color color)
		{
			// Performance Note:
			// Setting a background on Root performs atrociously! The background must be redrawn each frame.
			//   (Also it allocates a software surface). So this must be implemented through sprite-drawing.

			// So use a "sprite" to clear the screen:
			if(clearSprite == null)
			{
				clearSprite = new ClearSprite(true, Viewport.Width, Viewport.Height);
				AddSprite(clearSprite);
			}

			clearSprite.SetColor(color);
			DrawSprite(clearSprite);
		}

		#endregion


		#region Disposal

		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			IsDisposed = true;
		}

		#endregion



		#region Silverlight

		/// <summary>The display area of the game.</summary>
		public Canvas Root { get; internal set; }


		/// <summary>
		/// Adds a clipping rectangle around the game area.
		/// 
		/// PERFORMANCE WARNING: This performs extremely badly if there are any software surfaces in the game!
		/// </summary>
		public void HintClipEnable()
		{
			var rg = new RectangleGeometry();
			rg.Rect = new Rect(0, 0, Viewport.Width, Viewport.Height);
			Root.Clip = rg;

			//
			// Performance problems caused by setting a clipping rectangle:
			//
			// - An additional full-window software surface is created (presumably for a mask for the clip rectangle)
			// - All software regions redraw their entire area, instead of just their changed area
			// - Some hardware surfaces (possibly related to their position in the Z-order compared to software
			//     surfaces) have are allocated at full-window size, wasting enormous amounts of video memory!
			//
			// (Note: Some of these performance issues can also be caused by setting a background on the root element)
			// 
		}

		public void HintClipDisable()
		{
			Root.Clip = null;
		}

		#endregion


		#region Silverlight Sprite Rendering

		int currentFrameNumber = 0;
		int currentZIndex = 0;

		public int CurrentSpriteFrameNumber { get { return currentFrameNumber; } }

		List<Sprite> usedThisFrame = new List<Sprite>();
		List<Sprite> usedLastFrame = new List<Sprite>();

		public void DrawSprite(Sprite sprite)
		{
			sprite.AddToRoot.Visibility = Visibility.Visible;
			Canvas.SetZIndex(sprite.AddToRoot, ++currentZIndex);

			sprite.usedOnFrame = currentFrameNumber;
			usedThisFrame.Add(sprite);
		}

		public void AddSprite(Sprite sprite)
		{
			Root.Children.Add(sprite.AddToRoot);
			sprite.AddToRoot.Visibility = Visibility.Collapsed;
		}

		internal void EndSpriteFrame()
		{
			// Hide sprites that were used last frame but not this one
			foreach(Sprite sprite in usedLastFrame)
			{
				if(sprite.usedOnFrame != currentFrameNumber)
					sprite.AddToRoot.Visibility = Visibility.Collapsed;
			}
			usedLastFrame.Clear();

			unchecked { ++currentFrameNumber; }
			currentZIndex = 0;

			// Reuse lists by swapping
			List<Sprite> swap = usedThisFrame;
			usedThisFrame = usedLastFrame;
			usedLastFrame = swap;
		}

		#endregion

	}
}

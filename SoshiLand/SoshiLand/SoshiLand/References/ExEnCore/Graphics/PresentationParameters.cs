using System;

namespace Microsoft.Xna.Framework.Graphics
{
	public class PresentationParameters
	{
		public int BackBufferWidth { get; set; }
		public int BackBufferHeight { get; set; }
		public Rectangle Bounds { get { return new Rectangle(0, 0, BackBufferWidth, BackBufferHeight); } }

		public bool IsFullScreen { get; set; }

		public DisplayOrientation DisplayOrientation { get; set; }


		#region ExEn iOS Scaler Information
#if MONOTOUCH
		/// <summary>UI orientation of the device</summary>
		public MonoTouch.UIKit.UIInterfaceOrientation iOSInterfaceOrientation { get; set; }
#endif
		#endregion

	}
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace Microsoft.Xna.Framework.Media
{
	public class Song : IDisposable
	{
		GraphicsDevice device;

		string uri;
		internal SoundEffectInstance sound = null;

		internal Song(ContentManager content, string uri)
		{
			this.uri = uri;

			device = ((IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
		}


		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			if(IsDisposed)
				throw new ObjectDisposedException(this.ToString());

			IsDisposed = true;

			if(sound != null)
				sound.Dispose();
		}


		#region Preloading

		static List<Song> preloading = new List<Song>();
		static Queue<Song> preloadQueue = new Queue<Song>();

		private void PumpPreloadQueue()
		{
			if(preloading.Count == 0)
			{
				while(preloadQueue.Count > 0)
				{
					Song s = preloadQueue.Dequeue();
					if(s.sound == null && !s.IsDisposed)
					{
						HintBeginPreload();
						break;
					}
				}
			}
		}


		void CheckFinishedDownload()
		{
			if(sound.mediaElement.DownloadProgress > 0.99) // Download complete
			{
				DonePreloading();
			}
		}

		void DonePreloading()
		{
			preloading.Remove(this);
			PumpPreloadQueue();
		}

		bool downloadFailed = false;

		public void HintBeginPreload()
		{	
			if(sound == null)
			{
				sound = new SoundEffectInstance(device.Root);
				preloading.Add(this);
				sound.mediaElement.DownloadProgressChanged += (o, e) => CheckFinishedDownload();
				sound.mediaElement.MediaFailed += (o, e) => { downloadFailed = true; DonePreloading(); };
				sound.SetSourceForSong(uri);
				CheckFinishedDownload(); // just in case :)
			}
		}

		public void HintQueuePreload()
		{
			if(sound == null)
			{
				preloadQueue.Enqueue(this);
				PumpPreloadQueue();
			}
		}

		#endregion


	}
}

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Resources;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WaveMSS;

namespace Microsoft.Xna.Framework.Audio
{
	public class SoundEffect : IDisposable
	{
		#region Constructor and Data

		GraphicsDevice device;

		bool isWav = false;
		byte[] soundBuffer;

		internal SoundEffect(ContentManager content, string assetName)
		{
			Stream s = content.GetAssetStream(assetName, ".mp3");
			if(s == null)
			{
				s = content.GetAssetStream(assetName, ".wma");
			}
			if(s == null)
			{
				s = content.GetAssetStream(assetName, ".wav");
				if(s != null)
				{
					isWav = true;
				}
			}

			if(s == null)
				throw new ContentLoadException("Could not load audio asset: " + assetName);

			soundBuffer = new byte[s.Length];
			s.Read(soundBuffer, 0, soundBuffer.Length);
			s.Close();

			device = ((IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
		}

		public void Dispose()
		{
			DisposeFireAndForgetQueue();
		}

		#endregion


		public SoundEffectInstance CreateInstance()
		{
			SoundEffectInstance instance;
			MemoryStream stream = new MemoryStream(soundBuffer);
			if(isWav)
				instance = new SoundEffectInstance(device.Root, new WaveMediaStreamSource(stream));
			else
				instance = new SoundEffectInstance(device.Root, stream);

			return instance;
		}


		#region Fire-and-Forget Sounds (Shared Code!)
		// THIS CODE IS SHARED BETWEEN MONOTOUCH AND SILVERLIGHT
		// TODO FIXME: it really shouldn't be!!!!

		Queue<SoundEffectInstance> fireAndForgetQueue = new Queue<SoundEffectInstance>();

		public bool Play()
		{
			return Play(1, 0, 0);
		}

		public bool Play(float volume, float pitch, float pan)
		{
			SoundEffectInstance instance = null;

			if(fireAndForgetQueue.Count > 0)
			{
				SoundEffectInstance i = fireAndForgetQueue.Peek();
				if(i.ReadyToReFire)
				{
					instance = fireAndForgetQueue.Dequeue();
					instance.Stop(); // Rewind
				}
			}

			if(instance == null)
				instance = CreateInstance();

			instance.Volume = volume;
			instance.Play();

			fireAndForgetQueue.Enqueue(instance);

			return true;
		}

		void DisposeFireAndForgetQueue()
		{
			foreach(var instance in fireAndForgetQueue)
				instance.Dispose();
			fireAndForgetQueue.Clear();
		}

		#endregion

	}
}

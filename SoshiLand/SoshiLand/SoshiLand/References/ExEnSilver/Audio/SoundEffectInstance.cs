using System;
using System.Windows.Controls;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Media;
using System.IO;
using System.Diagnostics;
using WaveMSS;

namespace Microsoft.Xna.Framework.Audio
{
	public enum SoundState
	{
		Playing=0,
		Paused=1,
		Stopped=2
	}

	public class SoundEffectInstance : IDisposable
	{
		internal MediaElement mediaElement;

		// MediaElement must be on canvas for properties to change after SetSource
		bool mediaElementIsOnCanvas = false;
		bool mediaElementHasSourceSet = false;
		Canvas canvas;

		// Possible data sources
		WaveMediaStreamSource wavSource = null;
		Stream streamSource = null;


		#region Internal and Private functionality

		internal SoundEffectInstance(Canvas canvas, WaveMediaStreamSource source) : this(canvas, source, null) { }
		internal SoundEffectInstance(Canvas canvas, Stream source) : this(canvas, null, source) { }

		private SoundEffectInstance(Canvas canvas, WaveMediaStreamSource wavSource, Stream streamSource)
		{
			this.canvas = canvas;
			this.wavSource = wavSource;
			this.streamSource = streamSource;
			mediaElement = new MediaElement();

			this.Volume = 1f; // Matches XNA Behaviour
		}


		// Special constructor for Song - start pre-loading the audio
		internal SoundEffectInstance(Canvas canvas) : this(canvas, null, null) { }

		internal void SetSourceForSong(string uriSource)
		{
			mediaElementHasSourceSet = true;
			mediaElement.AutoPlay = false; // Stop the song auto playing
			mediaElement.Source = new Uri(uriSource, UriKind.RelativeOrAbsolute);
			MediaElementAddToCanvas(); // Can't be bothered figuring out what needs to be done on-canvas for Song
		}


		void MediaElementSetSource()
		{
			Debug.Assert(!mediaElementHasSourceSet);
			Debug.Assert((wavSource != null) != (streamSource != null)); // XOR

			if(wavSource != null)
				mediaElement.SetSource(wavSource);
			else if(streamSource != null)
				mediaElement.SetSource(streamSource);

			mediaElementHasSourceSet = true;
		}

		void MediaElementAddToCanvas()
		{
			if(!mediaElementIsOnCanvas)
				canvas.Children.Add(mediaElement);
			mediaElementIsOnCanvas = true;
		}

		#endregion


		#region Play Controls (XNA API)

		public void Play()
		{
			if(IsDisposed)
				throw new ObjectDisposedException(this.ToString());
			if(!mediaElementHasSourceSet)
				MediaElementSetSource();

			mediaElement.Play();
		}

		public void Resume()
		{
			if(IsDisposed)
				throw new ObjectDisposedException(this.ToString());
			if(!mediaElementHasSourceSet)
				MediaElementSetSource();

			mediaElement.Play();
		}

		public void Stop()
		{
			if(IsDisposed)
				throw new ObjectDisposedException(this.ToString());
			
			mediaElement.Stop();
		}

		public void Pause()
		{
			if(IsDisposed)
				throw new ObjectDisposedException(this.ToString());

			mediaElement.Pause();
		}

		#endregion


		#region Sound State (XNA API)

		private float _volume;
		public float Volume
		{
			get { return _volume; }
			set
			{
				if(IsDisposed)
					throw new ObjectDisposedException(this.ToString());

				if(_volume == value) // try to avoid adding to the canvas
					return;

				// IMPORTANT NOTE:
				// Changes to Volume after SetSource is called seem to be ignored if the
				// MediaElement is not added to the GraphicsDevice.Root (Silverlight tree)
				// Probably because it has whacky updating behaviour (dependency property stuff)
				// Calling SetSource again will cause the volume to update again
				// (However calling SetSource will Close() the previous stream,
				//  so re-setting the source won't work)
				// Adding to the Silverlight tree will allow the property to change
				// (but removing from the tree afterwards causes issues)

				if(mediaElementHasSourceSet && !mediaElementIsOnCanvas)
					MediaElementAddToCanvas();


				_volume = MathHelper.Clamp(value, 0, 1);


				// Silverlight tries to be "clever" and turns off sounds that have no volume
				// By setting a minimum volume of 0.02, Silverlight is forced to keep the sound ready


				// Polynomial approximation for for XNA->Silverlight volume calculated in Excel with this data:
				//   V	XNA	SL
				// 0.0	0	0
				// 0.1	11	1
				// 0.2	23	7
				// 0.3	35	16
				// 0.4	46	25
				// 0.5	58	37
				// 0.6	70	50
				// 0.7	82	65
				// 0.8	94	82
				// 0.9	105	99
				// 1.0	117	117
				// (pixel values of the Windows sound mixer, note that XNA is linear, y = x)

				float v = _volume;

				// Actual formula from Excel:
				//v = -2.6293f * (v*v*v*v) + 6.3524f * (v*v*v) - 5.4862f * (v*v) + 2.7394f * v + 0.02f;

				// Tweaked formula to get a range of values from 0.02 to 1.0:
				v = -2.6293f * (v*v*v*v) + 6.3555f * (v*v*v) - 5.4862f * (v*v) + 2.74f * v + 0.02f;

				mediaElement.Volume = v;

				// TODO: implement SoundEffect.MasterVolume
			}
		}

		public bool _looped;
		public bool IsLooped
		{
			get { return _looped; }
			set
			{
				if(IsDisposed)
					throw new ObjectDisposedException(this.ToString());

				if(mediaElementHasSourceSet)
					throw new InvalidOperationException("Cannot set IsLooped after a sound has been played.");

				if(wavSource != null)
				{
					wavSource.Loop = value;
				}
				else
				{
					if(value)
						throw new NotImplementedException("Cannot loop non-WAV files at the moment.");
				}
			}
		}

		public SoundState State
		{
			get
			{
				switch(mediaElement.CurrentState)
				{
					default:
					case MediaElementState.Stopped:
						return SoundState.Stopped;

					case MediaElementState.Paused:
						return SoundState.Paused;

					case MediaElementState.Buffering:
					case MediaElementState.Playing:
						return SoundState.Playing;
				}
			}
		}


		// For fire-and-forget sounds
		internal bool ReadyToReFire
		{
			get
			{
				SoundState state = State;
				return state == SoundState.Stopped || state == SoundState.Paused;
			}
		}

		#endregion


		#region Disposal

		bool IsDisposed
		{
			get { return mediaElement == null; }
		}

		public void Dispose()
		{
			if(IsDisposed)
				throw new ObjectDisposedException(this.ToString());

			if(mediaElementHasSourceSet)
				Stop();

			if(mediaElementIsOnCanvas)
				canvas.Children.Remove(mediaElement);
			mediaElement = null;
			mediaElementIsOnCanvas = false;

			if(wavSource != null)
				wavSource.Dispose();
			if(streamSource != null)
				streamSource.Dispose();
		}

#if DEBUG
		~SoundEffectInstance()
		{
			if(mediaElementIsOnCanvas)
				Debug.WriteLine("SoundEffectInstance was not disposed."); // probably not an issue during shutdown...
		}
#endif

		#endregion

	}
}

using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Xna.Framework.Media
{
	public static class MediaPlayer
	{
		public static bool GameHasControl { get { return true; } }

		static Song currentSong;


		#region Play Controls

		public static void Play(Song song)
		{
			Stop();

			currentSong = song;
			currentSong.HintBeginPreload(); // creates currentSong.sound
			// If the download hasn't fully connected, Play will do nothing. AutoPlay will cause the song to start when it is ready:
			currentSong.sound.mediaElement.AutoPlay = true;
			SetSongVolume();
			if(isRepeating)
				SetSongRepeatHandler();
			currentSong.sound.Play();
		}

		public static void Pause()
		{
			if(currentSong != null && currentSong.sound != null)
			{
				currentSong.sound.mediaElement.AutoPlay = false;
				currentSong.sound.Pause();
			}
		}

		public static void Resume()
		{
			if(currentSong != null && currentSong.sound != null)
			{
				currentSong.sound.Resume();
				currentSong.sound.mediaElement.AutoPlay = true;
			}
		}

		public static void Stop()
		{
			if(currentSong != null && currentSong.sound != null)
			{
				currentSong.sound.mediaElement.AutoPlay = false;
				ClearSongRepeatHandler();
				currentSong.sound.Stop();
			}
			currentSong = null;
		}

		#endregion


		#region Volume handling

		static bool isMuted = false;
		public static bool IsMuted
		{
			get { return isMuted; }
			set
			{
				isMuted = value;
				SetSongVolume();
			}
		}

		static float volume = 1f;
		public static float Volume
		{
			get { return volume; }
			set
			{
				volume = value;
				SetSongVolume();
			}
		}

		static void SetSongVolume()
		{
			if(currentSong != null)
				currentSong.sound.Volume = isMuted ? 0f : volume;
		}

		#endregion


		#region IsRepeating

		static bool isRepeating = false;
		public static bool IsRepeating
		{
			get { return isRepeating; }
			set
			{
				if(isRepeating = value)
					SetSongRepeatHandler();
				else
					ClearSongRepeatHandler();
			}
		}

		static bool songHasRepeatHandler = false;

		static void SetSongRepeatHandler()
		{
			if(currentSong != null && !songHasRepeatHandler)
			{
				currentSong.sound.mediaElement.MediaEnded += MediaEndedHandler;
				songHasRepeatHandler = true;
			}
		}

		static void ClearSongRepeatHandler()
		{
			if(currentSong != null && songHasRepeatHandler)
			{
				currentSong.sound.mediaElement.MediaEnded -= MediaEndedHandler;
				songHasRepeatHandler = false;
			}
		}

		static void RestartMedia(object sender, RoutedEventArgs e)
		{
			MediaElement element = (sender as MediaElement);
			element.Stop();
			element.Play();
		}

		static RoutedEventHandler MediaEndedHandler = new RoutedEventHandler(RestartMedia);

		#endregion

	}
}

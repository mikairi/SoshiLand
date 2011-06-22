using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Imaging;

namespace ExEnSilver.Graphics
{
	public static class ImageLoader
	{
		static int loadCount = 0;

		public static bool IsBusy
		{
			get
			{
				return loadCount > 0;
			}
		}

		static public BitmapImage GetBitmapImage(Stream source)
		{
			BitmapImage img = new BitmapImage();
			img.CreateOptions = BitmapCreateOptions.None;
			img.SetSource(source);
			img.ImageOpened += new EventHandler<RoutedEventArgs>(img_ImageOpened);
			img.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(img_ImageFailed);
			loadCount++;
			return img;
		}

		static void img_ImageFailed(object sender, ExceptionRoutedEventArgs e)
		{
			BitmapImage img = sender as BitmapImage;
			img.ImageFailed -= new EventHandler<ExceptionRoutedEventArgs>(img_ImageFailed);
			img.ImageOpened -= new EventHandler<RoutedEventArgs>(img_ImageOpened);
			loadCount--;
		}

		static void img_ImageOpened(object sender, RoutedEventArgs e)
		{
			BitmapImage img = sender as BitmapImage;
			img.ImageFailed -= new EventHandler<ExceptionRoutedEventArgs>(img_ImageFailed);
			img.ImageOpened -= new EventHandler<RoutedEventArgs>(img_ImageOpened);
			loadCount--;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Microsoft.Xna.Framework.Content
{

	public class ContentManager : IDisposable
	{
		#region Private Members

		private Dictionary<string, object> assets;
		private bool disposed;
		private string rootDirectory;
		private IServiceProvider serviceProvider;
		
		#endregion


		#region Public Properties

		public string RootDirectory
		{
			get
			{
				return rootDirectory;
			}
			set
			{
				rootDirectory = value;
			}
		}

		public IServiceProvider ServiceProvider
		{
			get { return this.serviceProvider; }
		}

		#endregion


		#region Public Constructors

		public ContentManager(IServiceProvider serviceProvider)
			: this(serviceProvider, string.Empty)
		{
		}

		public ContentManager(IServiceProvider serviceProvider, string rootDirectory)
		{
			if (serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");

			if (rootDirectory == null)
				throw new ArgumentNullException("rootDirectory");

			this.assets = new Dictionary<string, object>();
			this.rootDirectory = rootDirectory;
			this.serviceProvider = serviceProvider;
		}

		#endregion


		#region Destructor

		~ContentManager()
		{
			Dispose(false);
		}

		#endregion


		#region Protected Methods

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					// Dispose any managed resources
					Unload();
				}
				// Dispose any unmanaged resources

				this.disposed = true;
			}
		}


		protected virtual Stream OpenStream(string assetName)
		{
			Stream stream;
			try
			{
				string path = Path.Combine(rootDirectory, assetName + ".xnb");
				stream = File.OpenRead(path);
			}
			catch (FileNotFoundException fileNotFound)
			{
				throw new ContentLoadException("The content file was not found.", fileNotFound);
			}
			catch (DirectoryNotFoundException directoryNotFound)
			{
				throw new ContentLoadException("The directory was not found.", directoryNotFound);
			}
			catch (Exception exception)
			{
				throw new ContentLoadException("Opening stream error.", exception);
			}
			return stream;
		}

		#endregion Protected Methods


		#region Public Methods

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}


		Texture2D LoadTexture(string assetName)
		{
			Vector2 size = Vector2.Zero;

			// TODO: should this stream be closed, perhaps?
			Stream stream;
			if((stream = GetAssetStream(assetName, ".png")) == null)
				stream = GetAssetStream(assetName, ".jpg");
			
			GraphicsDevice graphicsDevice = ((IGraphicsDeviceService)this.serviceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
			Texture2D texture = new Texture2D(stream, graphicsDevice);

			return texture;
		}

		public virtual T Load<T>(string assetName)
		{
			assetName = assetName.Replace("\\", "/");
			object obj = null;
			if(assets.ContainsKey(assetName))
			{
				try
				{
					T asset = (T)assets[assetName];
					return asset;
				}
				catch
				{
				}
			}
			if(typeof(T) == typeof(Texture2D))
			{
				obj = LoadTexture(assetName);
			}
			else if(typeof(T) == typeof(SpriteFont))
			{
				obj = LoadSpriteFont(assetName);
			}
			else if(typeof(T) == typeof(SoundEffect))
			{
				obj = new SoundEffect(this, assetName);				
			}
			else if(typeof(T) == typeof(Song))
			{
				obj = new Song(this, GetAssetUri(assetName, ".mp3"));
			}
			if(obj != null)
			{
				assets.Add(assetName, obj);
			}
			return (T)obj;
		}
 
		public virtual void Unload()
		{
			if (this.disposed)
				throw new ObjectDisposedException(this.GetType().ToString());

			foreach (object o in assets)
			{
				IDisposable disposableObject = o as IDisposable;
				if (disposableObject != null)
					disposableObject.Dispose();
			}

			assets.Clear();
		}

		#endregion


		#region Fonts

		static Dictionary<string, SpriteFont> fontTranslationList = new Dictionary<string, SpriteFont>();

		public static void SilverlightFontTranslation(string assetName, SpriteFont font)
		{
			fontTranslationList[assetName] = font;
		}

		private SpriteFont LoadSpriteFont(string assetName)
		{
			SpriteFont font = null;
			fontTranslationList.TryGetValue(assetName, out font);

			if(font == null)
			{
				Texture2D texture = LoadTexture(assetName + "-exenfont");
				using(Stream metricsDataStream = GetAssetStream(assetName, "-exenfont.exenfont"))
				{
					font = new SpriteFontBitmap(texture, metricsDataStream, 1f);
				}
			}

			return font;
		}


		#endregion


		#region Private Methods

		internal string GetAssetUri(string assetName, string extension)
		{
			string path = Path.Combine(this.rootDirectory, assetName + extension);
			path = path.Replace("\\", "/");
			return "/" + path;
		}

		internal Stream GetAssetStream(string assetName, string extension)
		{
			string path = Path.Combine(this.rootDirectory, assetName + extension);
			path = path.Replace("\\", "/");

			if(path.StartsWith("./")) // Remove leading dot directory
				path = path.Substring(2);

			// Application.GetResourceStream looks in the XAP
			// (and assemblies in the XAP with a /{shortAssemblyName};component/ URI)

			// This differs from the behaviour of Source URIs as described here:
			// http://nerddawg.blogspot.com/2008/03/silverlight-2-demystifying-uri.html
			// normally a leading slash is required (and will fall back to looking beside the XAP)

			// By using GetResourceStream, content is forced to be loaded into memory
			// immediately and synchronously.

			StreamResourceInfo info = Application.GetResourceStream(new Uri(path, UriKind.Relative));
			return info != null ? info.Stream : null;
		}

		#endregion
	}
}

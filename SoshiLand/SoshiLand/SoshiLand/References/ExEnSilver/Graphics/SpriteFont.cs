using System;
using System.Text;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.Graphics
{
	public abstract class SpriteFont
	{

		#region Public API

		public abstract Vector2 MeasureString(string text);
		public abstract Vector2 MeasureString(StringBuilder text);

		public abstract ReadOnlyCollection<char> Characters { get; }
		public abstract char? DefaultCharacter { get; set; }
		public abstract int LineSpacing { get; set; }
		public abstract float Spacing { get; set; }

		#endregion


		#region Internal API

		internal SpriteFont() { }

		internal abstract void InternalDrawString(SpriteBatch sb, StringBuilder text,
				Vector2 position, Color color, float rotation, Vector2 origin,
				Vector2 scale, SpriteEffects effects, float layerDepth);

		internal abstract void InternalDrawString(SpriteBatch sb, string text,
				Vector2 position, Color color, float rotation, Vector2 origin,
				Vector2 scale, SpriteEffects effects, float layerDepth);

		#endregion

	}
}

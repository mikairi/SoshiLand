using System;
using SWI=System.Windows.Input;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Xna.Framework.Input
{
	public static class Keyboard
	{
		#region Silverlight to XNA Key Conversion Table

		static Keys[] SilverlightToXna = new Keys[256];

		static void SetKeyConversion(SWI.Key slKey, Keys xnaKey)
		{
			Debug.Assert((int)slKey < 256);
			SilverlightToXna[(int)slKey] = xnaKey;
		}

		static Keys GetKeyConversion(SWI.Key slKey)
		{
			Debug.Assert((int)slKey < 256);
			return SilverlightToXna[(int)slKey];
		}

		static void InitializeKeyConversionTable()
		{
			SetKeyConversion(SWI.Key.A, Keys.A);
			SetKeyConversion(SWI.Key.B, Keys.B);
			SetKeyConversion(SWI.Key.C, Keys.C);
			SetKeyConversion(SWI.Key.D, Keys.D);
			SetKeyConversion(SWI.Key.E, Keys.E);
			SetKeyConversion(SWI.Key.F, Keys.F);
			SetKeyConversion(SWI.Key.G, Keys.G);
			SetKeyConversion(SWI.Key.H, Keys.H);
			SetKeyConversion(SWI.Key.I, Keys.I);
			SetKeyConversion(SWI.Key.J, Keys.J);
			SetKeyConversion(SWI.Key.K, Keys.K);
			SetKeyConversion(SWI.Key.L, Keys.L);
			SetKeyConversion(SWI.Key.M, Keys.M);
			SetKeyConversion(SWI.Key.N, Keys.N);
			SetKeyConversion(SWI.Key.O, Keys.O);
			SetKeyConversion(SWI.Key.P, Keys.P);
			SetKeyConversion(SWI.Key.Q, Keys.Q);
			SetKeyConversion(SWI.Key.R, Keys.R);
			SetKeyConversion(SWI.Key.S, Keys.S);
			SetKeyConversion(SWI.Key.T, Keys.T);
			SetKeyConversion(SWI.Key.U, Keys.U);
			SetKeyConversion(SWI.Key.V, Keys.V);
			SetKeyConversion(SWI.Key.W, Keys.W);
			SetKeyConversion(SWI.Key.X, Keys.X);
			SetKeyConversion(SWI.Key.Y, Keys.Y);
			SetKeyConversion(SWI.Key.Z, Keys.Z);
			SetKeyConversion(SWI.Key.Up, Keys.Up);
			SetKeyConversion(SWI.Key.Down, Keys.Down);
			SetKeyConversion(SWI.Key.Left, Keys.Left);
			SetKeyConversion(SWI.Key.Right, Keys.Right);
			SetKeyConversion(SWI.Key.Enter, Keys.Enter);
			SetKeyConversion(SWI.Key.Space, Keys.Space);
			SetKeyConversion(SWI.Key.Back, Keys.Back);
			SetKeyConversion(SWI.Key.Delete, Keys.Delete);
			SetKeyConversion(SWI.Key.Escape, Keys.Escape);

			SetKeyConversion(SWI.Key.D0, Keys.D0);
			SetKeyConversion(SWI.Key.D1, Keys.D1);
			SetKeyConversion(SWI.Key.D2, Keys.D2);
			SetKeyConversion(SWI.Key.D3, Keys.D3);
			SetKeyConversion(SWI.Key.D4, Keys.D4);
			SetKeyConversion(SWI.Key.D5, Keys.D5);
			SetKeyConversion(SWI.Key.D6, Keys.D6);
			SetKeyConversion(SWI.Key.D7, Keys.D7);
			SetKeyConversion(SWI.Key.D8, Keys.D8);
			SetKeyConversion(SWI.Key.D9, Keys.D9);

			SetKeyConversion(SWI.Key.F1, Keys.F1);
			SetKeyConversion(SWI.Key.F2, Keys.F2);
			SetKeyConversion(SWI.Key.F3, Keys.F3);
			SetKeyConversion(SWI.Key.F4, Keys.F4);
			SetKeyConversion(SWI.Key.F5, Keys.F5);
			SetKeyConversion(SWI.Key.F6, Keys.F6);
			SetKeyConversion(SWI.Key.F7, Keys.F7);
			SetKeyConversion(SWI.Key.F8, Keys.F8);
			SetKeyConversion(SWI.Key.F9, Keys.F9);
			SetKeyConversion(SWI.Key.F10, Keys.F10);
			SetKeyConversion(SWI.Key.F11, Keys.F11);
			SetKeyConversion(SWI.Key.F12, Keys.F12);

			SetKeyConversion(SWI.Key.NumPad0, Keys.NumPad0);
			SetKeyConversion(SWI.Key.NumPad1, Keys.NumPad1);
			SetKeyConversion(SWI.Key.NumPad2, Keys.NumPad2);
			SetKeyConversion(SWI.Key.NumPad3, Keys.NumPad3);
			SetKeyConversion(SWI.Key.NumPad4, Keys.NumPad4);
			SetKeyConversion(SWI.Key.NumPad5, Keys.NumPad5);
			SetKeyConversion(SWI.Key.NumPad6, Keys.NumPad6);
			SetKeyConversion(SWI.Key.NumPad7, Keys.NumPad7);
			SetKeyConversion(SWI.Key.NumPad8, Keys.NumPad8);
			SetKeyConversion(SWI.Key.NumPad9, Keys.NumPad9);

			SetKeyConversion(SWI.Key.Tab, Keys.Tab);
			SetKeyConversion(SWI.Key.CapsLock, Keys.CapsLock);
			SetKeyConversion(SWI.Key.PageUp, Keys.PageUp);
			SetKeyConversion(SWI.Key.PageDown, Keys.PageDown);
			SetKeyConversion(SWI.Key.End, Keys.End);
			SetKeyConversion(SWI.Key.Home, Keys.Home);
			SetKeyConversion(SWI.Key.Insert, Keys.Insert);
			SetKeyConversion(SWI.Key.Decimal, Keys.Decimal);

			SetKeyConversion(SWI.Key.Add, Keys.Add);
			SetKeyConversion(SWI.Key.Subtract, Keys.Subtract);
			SetKeyConversion(SWI.Key.Multiply, Keys.Multiply);
			SetKeyConversion(SWI.Key.Divide, Keys.Divide);

			// Just use the left XNA keys (XNA user code will generally check both left and right anyway)
			SetKeyConversion(SWI.Key.Shift, Keys.LeftShift);
			SetKeyConversion(SWI.Key.Ctrl, Keys.LeftControl);
			SetKeyConversion(SWI.Key.Alt, Keys.LeftAlt);

			// There are platform-specific keys (see SilverSprite source), but I'm ignoring those for now
		}

		static Keyboard()
		{
			InitializeKeyConversionTable();
		}
		
		#endregion


		#region Silverlight Key State Tracking

		static KeyboardState currentState;

		internal static void SetEventHandlersOn(FrameworkElement element)
		{
			element.KeyDown += (o, e) =>
			{
				Keys key = GetKeyConversion(e.Key);
				if(key != Keys.None)
					currentState.InternalSetKey(key);
			};

			element.KeyUp += (o, e) =>
			{
				Keys key = GetKeyConversion(e.Key);
				if(key != Keys.None)
					currentState.InternalClearKey(key);
			};

			element.LostFocus += (o, e) =>
			{
				currentState.InternalClearAllKeys();
			};
		}

		#endregion


		#region XNA API

		public static KeyboardState GetState()
		{
			return currentState;
		}

		#endregion
	}
}

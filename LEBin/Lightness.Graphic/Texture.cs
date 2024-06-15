using Lightness.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Lightness.Graphic
{
	public class Texture
	{
		public Texture2D T2D;

		public int Width;

		public int Height;

		private static bool ChangedTextSettings = true;

		private static Font F;

		private static Color TColor = Color.FromArgb(255, 255, 255, 255);

		private static int TSize = 16;

		private static string FontName = "Meiryo";

		private static string LatestText = "";

		private static Texture TextText = null;

		private static GraphicsDevice DefaultGEngine = null;

		public Texture(GraphicsDevice GEngine, int w, int h)
		{
			this.Width = w;
			this.Height = h;
			this.T2D = new Texture2D(GEngine, w, h, false, SurfaceFormat.Color);
			Debug.Log('I', "Graphic", "Created new Texture: {0}x{1}", new object[]
			{
				w,
				h
			});
		}

		public void Dispose()
		{
			this.T2D.Dispose();
		}

		public static void SetTextColor(int R, int G, int B)
		{
			Texture.TColor = Color.FromArgb(255, R, G, B);
		}

		public static void SetTextSize(int P)
		{
			Texture.TSize = P;
			Texture.ChangedTextSettings = true;
		}

		public static void SetFont(string FN)
		{
			Texture.FontName = FN;
			Texture.ChangedTextSettings = true;
		}

		public static void SetDefaultGEngine(Engine GE)
		{
			Texture.DefaultGEngine = GE.GEngine;
		}

		public static Texture CreateFromText(string DrawText)
		{
			if (Texture.DefaultGEngine == null)
			{
				Debug.Log('E', "Texture", "DefaultGEngine is not defined. Use SetDefaultGEngine() to set.", new object[0]);
				return null;
			}
			return Texture.CreateFromText(Texture.DefaultGEngine, DrawText);
		}

		public static Texture CreateFromText(GraphicsDevice GEngine, string DrawText)
		{
			if (Texture.ChangedTextSettings)
			{
				Texture.F = new Font(Texture.FontName, (float)Texture.TSize, FontStyle.Regular, GraphicsUnit.Pixel);
			}
			if (DrawText != Texture.LatestText)
			{
				Size size = TextRenderer.MeasureText(DrawText, Texture.F);
				int width = size.Width * 12 / 10;
				Bitmap image = new Bitmap(width, size.Height);
				Graphics graphics = Graphics.FromImage(image);
				graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				graphics.DrawString(DrawText, Texture.F, new SolidBrush(Texture.TColor), 0f, 0f);
				Texture.LatestText = DrawText;
				Texture.TextText = Texture.CreateFromBitmap(GEngine, image);
			}
			return Texture.TextText;
		}

		public static Texture CreateFromFile(string FileName)
		{
			if (Texture.DefaultGEngine == null)
			{
				Debug.Log('E', "Texture", "DefaultGEngine is not defined. Use SetDefaultGEngine() to set.", new object[0]);
				return null;
			}
			return Texture.CreateFromFile(Texture.DefaultGEngine, FileName);
		}

		public static Texture CreateFromFile(GraphicsDevice GEngine, string FileName)
		{
			Debug.Log('I', "Graphic", "Creating new Texture from \"{0}\"", new object[]
			{
				FileName
			});
			ContentStream stream = new ContentStream("./Data/Image/" + FileName);
			Bitmap image = (Bitmap)Image.FromStream(stream, true);
			return Texture.CreateFromBitmap(GEngine, image);
		}

		public static Texture CreateFromBitmap(Bitmap image)
		{
			if (Texture.DefaultGEngine == null)
			{
				Debug.Log('E', "Texture", "DefaultGEngine is not defined. Use SetDefaultGEngine() to set.", new object[0]);
				return null;
			}
			return Texture.CreateFromBitmap(Texture.DefaultGEngine, image);
		}

		public static Texture CreateFromBitmap(GraphicsDevice GEngine, Bitmap image)
		{
			try
			{
				int num = 4;
				byte[] array = new byte[image.Width * image.Height * num];
				Rectangle rect = new Rectangle
				{
					X = 0,
					Y = 0,
					Width = image.Width,
					Height = image.Height
				};
				BitmapData bitmapData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
				for (int i = 0; i < image.Height; i++)
				{
					for (int j = 0; j < image.Width; j++)
					{
						int num2 = j * num + bitmapData.Stride * i;
						int num3 = (i * image.Width + j) * num;
						for (int k = 0; k < num; k++)
						{
							array[num3 + k] = Marshal.ReadByte(bitmapData.Scan0, num2 + num - (1 + k) % num - 1);
						}
					}
				}
				image.UnlockBits(bitmapData);
				Texture texture = new Texture(GEngine, image.Width, image.Height);
				texture.T2D.SetData<byte>(array);
				image.Dispose();
				return texture;
			}
			catch
			{
				Debug.Log('W', "Graphic", "Failed to load image", new object[0]);
			}
			return null;
		}
	}
}

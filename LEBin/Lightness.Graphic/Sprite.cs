using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lightness.Graphic
{
	public class Sprite
	{
		private bool Started;

		private SpriteBatch SB;

		private GraphicsDevice GEngine;

		public Sprite(GraphicsDevice GE)
		{
			this.GEngine = GE;
			this.SB = new SpriteBatch(GE);
		}

		public void Draw(Texture texture, int x, int y)
		{
			this.Draw(texture, x, y, 255);
		}

		public void Draw(Texture texture, int x, int y, int Alpha)
		{
			if (!this.Started)
			{
				this.Start();
			}
			try
			{
				float scale = (float)Alpha / 255f;
				this.SB.Draw(texture.T2D, new Vector2
				{
					X = (float)x,
					Y = (float)y
				}, Color.White * scale);
			}
			catch
			{
			}
		}

		public void DrawEx(Texture texture, int dx, int dy, int Alpha, int sx, int sy, int sw, int sh)
		{
			if (!this.Started)
			{
				this.Start();
			}
			try
			{
				float scale = (float)Alpha / 255f;
				this.SB.Draw(texture.T2D, new Rectangle(dx, dy, sw, sh), new Rectangle?(new Rectangle(sx, sy, sw, sh)), Color.White * scale);
			}
			catch
			{
			}
		}


		public void DrawDegree(Texture texture, int x, int y, int Alpha, int Degree, int cx, int cy)
		{
			if (!this.Started)
			{
				this.Start();
			}
			try
			{
				float Radian = 0;
				if (Degree != 0) { //ÇŸÇ∆ÇÒÇ«ÇÃèÍçáÇOÇ»ÇÃÇ≈ÅAçÇë¨âª
					Radian = Degree / 57.295780f;
				}

				this.DrawRadian(texture, x, y, Alpha, Radian, cx, cy);
			}
			catch
			{
			}
		}
		public void DrawRadian(Texture texture, int x, int y, int Alpha, float Radian, int cx, int cy)
		{
			if (!this.Started)
			{
				this.Start();
			}
			try
			{
				float FAlpha = (float)Alpha / 255F;

				float scale = (float)Alpha / 255f;
				Vector2 XYPosition = new Vector2 { X = (float)x, Y = (float)y };
				Rectangle UVPosition = new Rectangle { X = 0, Y = 0, Width = texture.Width, Height = texture.Height };
				Vector2 Center = new Vector2 { X = (float)cx, Y = (float)cy };
				this.SB.Draw(texture.T2D, XYPosition, UVPosition, Microsoft.Xna.Framework.Color.White * Alpha, Radian, Center, 1.0f, SpriteEffects.None, 0);
			}
			catch
			{
			}
		}


		public void DrawText(string T, int x, int y)
		{
			this.DrawText(T, x, y, 255);
		}

		public void DrawText(string T, int x, int y, int Alpha)
		{
			Texture texture = Texture.CreateFromText(this.GEngine, T);
			this.Draw(texture, x, y, Alpha);
		}

		public void Start()
		{
			if (this.Started)
			{
				return;
			}
			this.Started = true;
			this.SB.Begin();
		}

		public void End()
		{
			if (!this.Started)
			{
				return;
			}
			this.Started = false;
			this.SB.End();
		}

		public void Dispose()
		{
			this.SB.Dispose();
		}
	}
}

using Lightness.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lightness.Graphic {
	public class Engine {
		public GraphicsDevice GEngine;

		public IntPtr HWND = IntPtr.Zero;

		public Engine(IntPtr WindowHandle) {
			this.HWND = WindowHandle;
			Debug.Log('I', "Graphic", "Initialize Graphic Engine", new object[0]);
			PresentationParameters presentationParameters = new PresentationParameters();
			presentationParameters.DeviceWindowHandle = WindowHandle;
			presentationParameters.IsFullScreen = false;
			try {
				GraphicsAdapter defaultAdapter = GraphicsAdapter.DefaultAdapter;
				this.GEngine = new GraphicsDevice(defaultAdapter, GraphicsProfile.HiDef, presentationParameters);
			} catch {
				try {
					GraphicsAdapter defaultAdapter2 = GraphicsAdapter.DefaultAdapter;
					this.GEngine = new GraphicsDevice(defaultAdapter2, GraphicsProfile.Reach, presentationParameters);
				} catch {
					Debug.Log('E', "Graphic", "Failed to initialize", new object[0]);
					throw new CantInitializeException();
				}
			}
			this.GEngine.BlendState = BlendState.AlphaBlend;
			this.Clear();
			this.Render();
		}

		public void Initialize(IntPtr WindowHandle) {
			Debug.Log('I', "Graphic", "Initialize Graphic Engine", new object[0]);
			PresentationParameters presentationParameters = new PresentationParameters();
			presentationParameters.DeviceWindowHandle = WindowHandle;
			presentationParameters.IsFullScreen = false;
			try {
				GraphicsAdapter defaultAdapter = GraphicsAdapter.DefaultAdapter;
				this.GEngine = new GraphicsDevice(defaultAdapter, GraphicsProfile.HiDef, presentationParameters);
			} catch {
				try {
					GraphicsAdapter defaultAdapter2 = GraphicsAdapter.DefaultAdapter;
					this.GEngine = new GraphicsDevice(defaultAdapter2, GraphicsProfile.Reach, presentationParameters);
				} catch {
					Debug.Log('E', "Graphic", "Failed to initialize", new object[0]);
					throw new CantInitializeException();
				}
			}
			this.GEngine.BlendState = BlendState.AlphaBlend;
			this.Clear();
			this.Render();
		}

		public void Render() {
			try {
				this.GEngine.Present();
			} catch {
				Debug.Log('E', "Graphic", "Failed to Rendering. Lost?", new object[0]);
			}
		}

		public void Clear() {
			this.GEngine.Clear(ClearOptions.Target, Color.Black, 1f, 0);
		}

		public Sprite CreateSprite() {
			return new Sprite(this.GEngine);
		}

		public Texture CreateTexture(int w, int h) {
			return new Texture(this.GEngine, w, h);
		}

		public Texture CreateTextureFromFile(string FileName) {
			return Texture.CreateFromFile(this.GEngine, FileName);
		}
	}
}

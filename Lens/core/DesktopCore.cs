using System;
using Lens.util;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.Core {
	public class DesktopCore : Core {
		private static bool resizing;

		public override void Init(int width, int height, bool fullscreen) {
			base.Init(width, height, fullscreen);
			
			Window.AllowUserResizing = true;
			Window.ClientSizeChanged += OnClientSizeChanged;

			if (fullscreen) {
				Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
				Graphics.IsFullScreen = true;
			} else {
				Graphics.PreferredBackBufferWidth = width;
				Graphics.PreferredBackBufferHeight = height;
				Graphics.IsFullScreen = false;
			}
		}

		public override void SetWindowed(int width, int height) {
			if (width > 0 && height > 0) {
				resizing = true;
				Graphics.PreferredBackBufferWidth = width;
				Graphics.PreferredBackBufferHeight = height;
				Graphics.IsFullScreen = false;
				Graphics.ApplyChanges();
				Log.Info("Going windowed " + width + ":" + height);
				resizing = false;
			}
		}

		public override void SetFullscreen() {
			resizing = true;
			Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			Graphics.IsFullScreen = true;
			Graphics.ApplyChanges();
			Log.Info("Going to fullscreen");
			resizing = false;
		}

		protected virtual void OnClientSizeChanged(object sender, EventArgs e) {
			if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !resizing) {
				resizing = true;

				Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				Engine.Instance.UpdateView();

				resizing = false;
			}
		}
	}
}
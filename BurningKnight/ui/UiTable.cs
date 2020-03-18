using System.Collections.Generic;
using BurningKnight.assets;
using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiTable : UiEntity {
		private const float EntryHeight = 15;
		private const float XPadding = 3;
		private const float YPadding = 1;

		private struct Entry {
			public string Key;
			public string Value;
			public float ValueWidth;
		}
		
		private List<Entry> entries = new List<Entry>();
		private TextureRegion texture;

		public void Add(string key, string value) {
			entries.Add(new Entry {
				Key = key,
				Value = value,
				ValueWidth = Font.Small.MeasureString(value).Width
			});
		}

		public void Prepare() {
			Clickable = false;
			Height = entries.Count * EntryHeight;

			texture = CommonAse.Ui.GetSlice("table_item");
		}

		public override void Render() {
			Graphics.Color.A = 200;
			
			for (var i = 0; i < entries.Count; i++) {
				Graphics.Render(texture, new Vector2(X, Y + i * EntryHeight));
			}

			Graphics.Color.A = 255;

			for (var i = 0; i < entries.Count; i++) {
				var entry = entries[i];
				var y = Y + i * EntryHeight + YPadding;

				Graphics.Print(entry.Key, Font.Small, new Vector2(X + XPadding, y));
				Graphics.Print(entry.Value, Font.Small, new Vector2(Right - entry.ValueWidth - XPadding, y));
			}
		}
	}
}
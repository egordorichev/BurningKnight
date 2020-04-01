using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.projectile;
using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiTable : UiEntity {
		private const float EntryHeight = 15;
		
		private List<UiTableEntry> entries = new List<UiTableEntry>();

		public override void AddComponents() {
			base.AddComponents();
			Width = 128;
		}

		public void Clear() {
			foreach (var e in entries) {
				e.Done = true;
			}
			
			entries.Clear();
		}

		public void Add(string key, string value, bool h = false) {
			value = value ?? "";

			var entry = new UiTableEntry() {
				Label = key ?? "",
				Value = value
			};

			if (h) {
				entry.Color = ProjectileColor.Cyan;
			}

			((UiPane) Super).Add(entry);
			entries.Add(entry);
		}

		public void Prepare() {
			Clickable = false;
			Height = entries.Count * EntryHeight;
			
		}

		public override void Update(float dt) {
			base.Update(dt);

			var p = ((UiPane) Super).Position;
			
			for (var i = 0; i < entries.Count; i++) {
				var e = entries[i];
				
				e.RelativeX = RelativeX;
				e.RelativeY = RelativeY + i * EntryHeight;
				e.Position = p + e.RelativePosition;
			}
		}

		public override void Render() {
			
		}
	}
}
using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.projectile;
using Lens.assets;
using Lens.graphics;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiTable : UiEntity {
		private const float EntryHeight = 15;
		
		private List<UiTableEntry> entries = new List<UiTableEntry>();

		public override void AddComponents() {
			base.AddComponents();
			Width = 128 + 16;
		}

		public void Clear() {
			foreach (var e in entries) {
				((UiPane) Super).Remove(e);
			}
			
			entries.Clear();
		}

		public void Add(string key, string value, bool h = false, Action<UiButton> a = null) {
			value = value ?? "";

			var entry = new UiTableEntry() {
				Label = key ?? "",
				Value = value,
				Click = a
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

				if (e.Hidden) {
					e.Hidden = false;
					e.scale = 0;

					Tween.To(1, 0, x => e.scale = x, Rnd.Float(0.3f, 0.5f));
				}
			}
		}

		public override void Render() {
			
		}
	}
}
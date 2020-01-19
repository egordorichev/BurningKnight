using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.graphics;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.item.renderer {
	public class AngledRenderer : ItemRenderer {
		public float Angle;
		public bool InvertBack;
		public float AddedAngle;
		public float SwingAngle;
		
		private double lastAngle;
		private float sx = 1;
		private float sy = 1;
		private float oy;
		private float ox;

		public override void Render(bool atBack, bool paused, float dt, bool shadow, int offset) {
			var s = dt * 10f;

			sx = 1;//sx += (1 - sx) * s;
			sy = 1;//sy += (1 - sy) * s;
			ox += (-ox) * s;
			oy += (-oy) * s;
			
			var region = Item.Region;
			var owner = Item.Owner;

			var of = owner.GraphicsComponent.Flipped;
			var flipped = false;
			
			var angle = MathUtils.Mod((of ? -Angle : Angle) + (atBack ? ((InvertBack ? -1 : 1) * (of ? -Math.PI / 4 : Math.PI / 4)) : lastAngle), Math.PI * 2);
			var vf = angle > Math.PI * 0.5f && angle < Math.PI * 1.5f;
			
			if (!atBack && !paused && !shadow) {
				var to = owner.GetComponent<AimComponent>().Aim;
				var dx = Nozzle.X - Origin.X;
				var dy = Nozzle.Y - Origin.Y;

				if (vf) {
					dy *= -1;
				}

				var a = MathUtils.Angle(dx, dy) + lastAngle - AddedAngle - SwingAngle;
				var d = MathUtils.Distance(dx, dy);
				to -= MathUtils.CreateVector(a, d);

				owner.GetComponent<AimComponent>().Aim = to;
				lastAngle = MathUtils.LerpAngle(lastAngle, owner.AngleTo(to), dt * 12f);
			}

			if (atBack) {
				flipped = !flipped;
			} else {
				angle += (SwingAngle + AddedAngle) * (of ? -1 : 1);
			}

			var pos = new Vector2(owner.CenterX + (of ? -5 : 5), owner.CenterY + offset + (shadow ? owner.Height : 0));
			var or = Origin + new Vector2(ox, oy);
			var sc = new Vector2(flipped ? -sx : sx, (shadow ^ vf) ? -sy : sy);
			var fangle = (float) angle * (shadow ? -1 : 1);

			if (!shadow && !atBack) {
				var dx = Nozzle.X - or.X;
				var dy = Nozzle.Y - or.Y;

				if (vf) {
					dy *= -1;
				}

				var a = MathUtils.Angle(dx, dy) + angle;
				var d = MathUtils.Distance(dx, dy);

				var aim = owner.GetComponent<AimComponent>();

				aim.Center = pos + MathUtils.CreateVector(a, d);

				d = (aim.Aim - pos).Length();
				aim.RealAim = aim.Center + MathUtils.CreateVector(angle - AddedAngle - SwingAngle, d);
			}

			if (Item.Scourged) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(1f);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ItemGraphicsComponent.ScourgedColor);

				foreach (var d in MathUtils.Directions) {
					Graphics.Render(region, pos + d, fangle, or, sc);				
				}
				
				Shaders.End();
			}

			Graphics.Render(region, pos, fangle, or, sc);
			Graphics.Print($"{sc.X} {sc.Y}", Font.Small, pos - new Vector2(32));
		}

		public override void OnUse() {
			base.OnUse();

			sx = 0.3f;
			sy = 2f;
			ox = 8;
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			InvertBack = settings["invert_back"].Bool(true);
			AddedAngle = settings["aa"].Number(0).ToRadians();
		}

		public static void RenderDebug(string id, JsonValue parent, JsonValue root) {
			ItemRenderer.RenderDebug(id, parent, root);

			var invert = root["invert_back"].AsBoolean;

			if (ImGui.Checkbox("Invert back?", ref invert)) {
				root["invert_back"] = invert;
			}
			
			var min = (float) root["aa"].Number(0);

			if (ImGui.InputFloat("Added Angle", ref min)) {
				root["aa"] = min;
			}
		}
	}
}
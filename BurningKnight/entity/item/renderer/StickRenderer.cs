using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using ImGuiNET;
using Lens.graphics;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.item.renderer {
	public class StickRenderer : ItemRenderer {
		private double lastAngle;
		private Vector2 scale = Vector2.One;
		private bool horizontal;
		private float move;
		private float currentMove;
		private float moveTime;
		private float returnTime;
		
		public override void OnUse() {
			if (move > 0.1f) {
				Tween.To(move, currentMove, x => currentMove = x, moveTime).OnEnd = () => {
					Tween.To(0, currentMove, x => currentMove = x, returnTime);
				};
			} else {
			
				scale.X = 1.4f;
				scale.Y = 0.3f;

				Tween.To(1, scale.X, x => scale.X = x, 0.2f);
				Tween.To(1, scale.Y, x => scale.Y = x, 0.2f);	
			}
		}

		public override void Render(bool atBack, bool paused, float dt, bool shadow, int offset) {
			var region = Item.Region;
			var owner = Item.Owner;
			
			if (!atBack && !paused) {
				var to = owner.GetComponent<AimComponent>().Aim;
				var dx = Nozzle.X - Origin.X;
				var dy = Nozzle.Y - Origin.Y;
				
				var a = MathUtils.Angle(dx, dy) + lastAngle;
				var d = MathUtils.Distance(dx, dy);

				to -= MathUtils.CreateVector(a, d);
				lastAngle = MathUtils.LerpAngle(lastAngle, owner.AngleTo(to) + Math.PI * 0.5f, dt * 6f);
			}

			var angle = atBack ? (float) Math.PI * (owner.GraphicsComponent.Flipped ? 0.25f : -0.25f) : (float) lastAngle;

			if (horizontal) {
				angle -= (float) Math.PI * 0.5f;
			}

			var pos = new Vector2(
				owner.CenterX + (horizontal ? 0 : (region.Width / 2f) * (owner.GraphicsComponent.Flipped ? -1 : 1)),
				owner.CenterY + offset + (shadow ? owner.Height : 0)
			);

			var or = Origin + new Vector2(0, currentMove);
			var fangle = shadow ? -angle : angle;
			var sc = new Vector2(scale.X, shadow ? -scale.Y : scale.Y);

			if (!shadow) {
				if (!atBack) {
					var dx = Nozzle.X - or.X;
					var dy = Nozzle.Y - or.Y;
					var a = MathUtils.Angle(dx, dy) + angle;
					var d = MathUtils.Distance(dx, dy);

					var aim = owner.GetComponent<AimComponent>();
					aim.Center = pos + MathUtils.CreateVector(a, d);

					d = (aim.Aim - pos).Length();
					aim.RealAim = aim.Center + MathUtils.CreateVector(angle - (horizontal ? 0 : Math.PI / 2), d);
				}
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
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			horizontal = settings["h"];
			move = settings["mv"].Number(0);
			moveTime = settings["mt"].Number(0.1f);
			returnTime = settings["rt"].Number(0.2f);
		}

		public static void RenderDebug(string id, JsonValue parent, JsonValue root) {			
			ItemRenderer.RenderDebug(id, parent, root);

			var h = root["h"].Bool(false);

			if (ImGui.Checkbox("Horizontal", ref h)) {
				root["h"] = h;
			}
			
			var mv = (float) root["mv"].Number(0);

			if (ImGui.InputFloat("Move", ref mv)) {
				root["mv"] = mv;
			}
			
			var mt = (float) root["mt"].Number(0.1f);

			if (ImGui.InputFloat("Move Time", ref mt)) {
				root["mt"] = mt;
			}
			
			var rt = (float) root["rt"].Number(0.2f);

			if (ImGui.InputFloat("Return Time", ref rt)) {
				root["rt"] = rt;
			}
		}
	}
}
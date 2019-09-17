using System;
using BurningKnight.entity.component;
using ImGuiNET;
using Lens.graphics;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

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
			var origin = horizontal ? new Vector2(0, region.Height / 2f) : new Vector2(region.Width / 2f, region.Height);
			
			if (!atBack && !paused) {
				lastAngle = MathUtils.LerpAngle(lastAngle, owner.AngleTo(owner.GetComponent<AimComponent>().Aim) + Math.PI * 0.5f, dt * 6f);
			}

			var angle = atBack ? (float) Math.PI * (owner.GraphicsComponent.Flipped ? 0.25f : -0.25f) : (float) lastAngle;

			if (horizontal) {
				angle -= (float) Math.PI * 0.5f;
			}
			
			Graphics.Render(region, new Vector2(
				                        owner.CenterX + (horizontal ? 0 : (region.Width / 2f) * (owner.GraphicsComponent.Flipped ? -1 : 1)), 
				                        owner.CenterY + offset
			), shadow ? -angle : angle, 
				origin + new Vector2(0, currentMove), new Vector2(scale.X, shadow ? -scale.Y : scale.Y));
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			horizontal = settings["h"];
			move = settings["mv"].Number(0);
			moveTime = settings["mt"].Number(0.1f);
			returnTime = settings["rt"].Number(0.2f);
		}

		public static void RenderDebug(string id, JsonValue parent, JsonValue root) {
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
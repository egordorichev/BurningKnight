using System;
using BurningKnight.assets;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens.graphics;
using Lens.input;
using Lens.lightJson;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.renderer {
	public class AngledRenderer : ItemRenderer {
		public float Angle;
		public Vector2 Origin;
		public bool InvertBack;
		
		private double lastAngle;
		private float sx = 1;
		private float sy = 1;
		private float oy;
		private float ox;

		public override void Render(bool atBack, bool paused, float dt) {
			float s = dt * 10f;
			
			sx += (1 - sx) * s;
			sy += (1 - sy) * s;
			ox += (-ox) * s;
			oy += (-oy) * s;
			
			var region = Item.Region;
			var owner = Item.Owner;
			
			var flipped = owner.GraphicsComponent.Flipped;
			
			if (!atBack && !paused) {
				lastAngle = MathUtils.LerpAngle(lastAngle, owner.AngleTo(Input.Mouse.GamePosition), dt * 6f);
			}
			
			var angle = (flipped ? -Angle : Angle) + (atBack ? ((InvertBack ? -1 : 1) * (flipped ? -Math.PI / 4 : Math.PI / 4)) : lastAngle);

			if (atBack) {
				flipped = !flipped;
			}

			Graphics.Render(region, new Vector2(owner.CenterX + (flipped ? -3 : 3), owner.CenterY), 
				(float) angle, Origin + new Vector2(ox, oy), new Vector2(flipped ? -sx : sx, sy));
		}

		public override void OnUse() {
			base.OnUse();

			sx = 0.3f;
			sy = 2f;
			ox = 8;
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			Origin.X = settings["ox"].Number(0);
			Origin.Y = settings["oy"].Number(0);
			InvertBack = settings["invert_back"].Bool(true);
		}

		private static bool snapGrid = true;

		public static unsafe void RenderDebug(string id, JsonValue parent, JsonValue root) {
			var v = new System.Numerics.Vector2((float) root["ox"].AsNumber * 3, (float) root["oy"].AsNumber * 3);
			var region = CommonAse.Items.GetSlice(id);
			var pos = ImGui.GetWindowPos() + ImGui.GetCursorPos();
			
			if (ImGui.IsMouseDown(1)) {
				v = ImGui.GetMousePos() - pos;

				if (snapGrid) {
					v.X = (float) (Math.Floor(v.X / 3) * 3);
					v.Y = (float) (Math.Floor(v.Y / 3) * 3);
				}
				
				v.X = VelcroPhysics.Utilities.MathUtils.Clamp(v.X, 0, region.Width * 3);
				v.Y = VelcroPhysics.Utilities.MathUtils.Clamp(v.Y, 0, region.Height * 3);
				
				root["ox"] = v.X / 3f;
				root["oy"] = v.Y / 3f;
			}
			
			ImGuiNative.ImDrawList_AddRect(ImGui.GetWindowDrawList(), pos - new System.Numerics.Vector2(1, 1), pos + new System.Numerics.Vector2(region.Width * 3 + 1, region.Height * 3 + 1), ColorUtils.WhiteColor.PackedValue, 0, 0, 1);
			ItemEditor.DrawItem(region);
			ImGuiNative.ImDrawList_AddCircleFilled(ImGui.GetWindowDrawList(), pos + v, 3, ColorUtils.WhiteColor.PackedValue, 8);

			v /= 3f;

			ImGui.Checkbox("Snap to grid", ref snapGrid);
			
			if (ImGui.InputFloat2("Origin", ref v)) {
				root["ox"] = v.X;
				root["oy"] = v.Y;
			}

			if (ImGui.Button("tx")) {
				root["ox"] = 0;
			}
			
			ImGui.SameLine();
			
			if (ImGui.Button("ty")) {
				root["oy"] = 0;
			}

			if (ImGui.Button("cx")) {
				root["ox"] = region.Width / 2f;
			}
			
			ImGui.SameLine();
			
			if (ImGui.Button("cy")) {
				root["oy"] = region.Height / 2f;
			}

			if (ImGui.Button("bx")) {
				root["ox"] = region.Width;
			}
			
			ImGui.SameLine();
			
			if (ImGui.Button("by")) {
				root["oy"] = region.Height;
			}

			var invert = root["invert_back"].AsBoolean;

			if (ImGui.Checkbox("Invert back", ref invert)) {
				root["invert_back"] = invert;
			}
		}
	}
}
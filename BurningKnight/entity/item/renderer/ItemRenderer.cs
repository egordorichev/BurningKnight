using System;
using BurningKnight.assets;
using BurningKnight.state;
using ImGuiNET;
using Lens.graphics;
using Lens.lightJson;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.renderer {
	public class ItemRenderer {
		public Item Item;
		public Vector2 Origin;
		public Vector2 Nozzle;
		
		public virtual void Render(bool atBack, bool paused, float dt, bool shadow, int offset) {
			
		}

		public virtual void Setup(JsonValue settings) {
			Origin.X = settings["ox"].Number(0);
			Origin.Y = settings["oy"].Number(0);
			Nozzle.X = settings["nx"].Number(0);
			Nozzle.Y = settings["ny"].Number(0);
		}

		public virtual void OnUse() {
			
		}
		
		private static bool snapGrid = true;

		public static unsafe void RenderDebug(string id, JsonValue parent, JsonValue root) {
			if (ImGui.TreeNode("Origin")) {
				var v = new System.Numerics.Vector2((float) root["ox"].AsNumber * 3, (float) root["oy"].AsNumber * 3);
				var region = CommonAse.Items.GetSlice(id);
				var m = ImGui.GetScrollY();
				var pos = ImGui.GetWindowPos() + ImGui.GetCursorPos() - new System.Numerics.Vector2(0, m);

				if (ImGui.IsMouseDown(1)) {
					v = ImGui.GetMousePos() - pos;
					
					if (!(v.X < 0) && !(v.Y < 0) && !(v.X > region.Width * 3) && !(v.Y > region.Height * 3)) {
						if (snapGrid) {
							v.X = (float) (Math.Floor(v.X / 3) * 3);
							v.Y = (float) (Math.Floor(v.Y / 3) * 3);
						}

						v.X = VelcroPhysics.Utilities.MathUtils.Clamp(v.X, 0, region.Width * 3);
						v.Y = VelcroPhysics.Utilities.MathUtils.Clamp(v.Y, 0, region.Height * 3);

						root["ox"] = v.X / 3f;
						root["oy"] = v.Y / 3f;
					}
				}
				
				ImGuiNative.ImDrawList_AddRect(ImGui.GetWindowDrawList(), pos - new System.Numerics.Vector2(1, 1),
					pos + new System.Numerics.Vector2(region.Width * 3 + 1, region.Height * 3 + 1),
					ColorUtils.WhiteColor.PackedValue, 0, 0, 1);

				ItemEditor.DrawItem(region);

				ImGuiNative.ImDrawList_AddCircleFilled(ImGui.GetWindowDrawList(), pos + v, 3, ColorUtils.WhiteColor.PackedValue,
					8);

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

				ImGui.TreePop();
			}
			
			if (ImGui.TreeNode("Nozzle")) {
				var v = new System.Numerics.Vector2((float) root["nx"].AsNumber * 3, (float) root["ny"].AsNumber * 3);
				var region = CommonAse.Items.GetSlice(id);
				var m = ImGui.GetScrollY();
				var pos = ImGui.GetWindowPos() + ImGui.GetCursorPos() - new System.Numerics.Vector2(0, m);
				
				if (ImGui.IsMouseDown(1)) {
					v = ImGui.GetMousePos() - pos;

					if (!(v.X < 0) && !(v.Y < 0) && !(v.X > region.Width * 3) && !(v.Y > region.Height * 3)) {
						if (snapGrid) {
							v.X = (float) (Math.Floor(v.X / 3) * 3);
							v.Y = (float) (Math.Floor(v.Y / 3) * 3);
						}

						v.X = VelcroPhysics.Utilities.MathUtils.Clamp(v.X, 0, region.Width * 3);
						v.Y = VelcroPhysics.Utilities.MathUtils.Clamp(v.Y, 0, region.Height * 3);

						root["nx"] = v.X / 3f;
						root["ny"] = v.Y / 3f;
					}
				}

				ImGuiNative.ImDrawList_AddRect(ImGui.GetWindowDrawList(), pos - new System.Numerics.Vector2(1, 1),
					pos + new System.Numerics.Vector2(region.Width * 3 + 1, region.Height * 3 + 1),
					ColorUtils.WhiteColor.PackedValue, 0, 0, 1);

				ItemEditor.DrawItem(region);

				ImGuiNative.ImDrawList_AddCircleFilled(ImGui.GetWindowDrawList(), pos + v, 3, ColorUtils.WhiteColor.PackedValue,
					8);

				v /= 3f;

				ImGui.Checkbox("Snap to grid", ref snapGrid);

				if (ImGui.InputFloat2("Nozzle", ref v)) {
					root["nx"] = v.X;
					root["ny"] = v.Y;
				}

				if (ImGui.Button("tx")) {
					root["nx"] = 0;
				}

				ImGui.SameLine();

				if (ImGui.Button("ty")) {
					root["ny"] = 0;
				}

				if (ImGui.Button("cx")) {
					root["nx"] = region.Width / 2f;
				}

				ImGui.SameLine();

				if (ImGui.Button("cy")) {
					root["ny"] = region.Height / 2f;
				}

				if (ImGui.Button("bx")) {
					root["nx"] = region.Width;
				}

				ImGui.SameLine();

				if (ImGui.Button("by")) {
					root["oy"] = region.Height;
				}

				ImGui.TreePop();
			}
		}
	}
}
using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.state;
using BurningKnight.state.save;
using ImGuiNET;
using Lens;
using Lens.game;
using Lens.graphics.gamerenderer;

namespace BurningKnight.ui.imgui {
	public static class DebugWindow {
		private static string[] states = {
			"ingame", "dialog_editor", "level_editor", "pico", "load", "save_explorer", "room_editor"
		};

		private static Type[] types = {
			typeof(InGameState), typeof(DialogEditorState),
			typeof(EditorState), typeof(PicoState), typeof(LoadState),
			typeof(SaveExplorerState), typeof(RoomEditorState)
		};
		
		public static void Render() {
			if (!ImGui.Begin("Debug", ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				return;
			}
			
			ImGui.Text($"FPS: {Engine.Instance.Counter.CurrentFramesPerSecond}");

			var current = 0;
			var t = Engine.Instance.State.GetType();
			
			for (var i = 0; i < types.Length; i++) {
				if (t == types[i]) {
					current = i;
					break;
				}
			}

			ImGui.DragFloat("Speed", ref Engine.Instance.Speed, 0.01f, 0.1f, 2f);
			
			ImGui.Text($"Draw calls: {Engine.Graphics.GraphicsDevice.Metrics.DrawCount}");
			ImGui.Text($"Clear calls: {Engine.Graphics.GraphicsDevice.Metrics.ClearCount}");
			ImGui.Text($"FBO binds: {Engine.Graphics.GraphicsDevice.Metrics.TargetCount}");
			ImGui.Text($"Shader binds: {Engine.Graphics.GraphicsDevice.Metrics.PixelShaderCount}");
			ImGui.Text($"Texture count: {Engine.Graphics.GraphicsDevice.Metrics.TextureCount}");
			ImGui.Spacing();
			ImGui.Checkbox("Enable batcher", ref GameRenderer.EnableBatcher);
			
			ImGui.Separator();
			
			var old = current;

			if (ImGui.Combo("State", ref current, states, states.Length) && old != current) {
				if (current == 0) {
					Engine.Instance.SetState(new LoadState());
				} else {
					Engine.Instance.SetState((GameState) Activator.CreateInstance(types[current]));
				}
			}

			ImGui.Separator();
			
			ImGui.Text($"Kills: {Run.KillCount}");
			ImGui.Text($"Time: {Run.FormatTime()}");
			ImGui.Text($"Has run: {Run.HasRun}");

			if (ImGui.Button("Go to hall (0)")) {
				Run.Depth = 0;
			}
			
			ImGui.SameLine();
			
			if (ImGui.Button("Go to hub (-1)")) {
				Run.Depth = -1;
			}
			
			if (ImGui.Button("New run")) {
				Run.StartNew();
			}
			
			ImGui.Separator();

			if (Run.Level != null) {
				var player = LocalPlayer.Locate(Run.Level.Area);

				if (player != null) {
					ImGui.Checkbox("Unhittable", ref player.GetComponent<HealthComponent>().Unhittable);
				}
			}

			ImGui.End();
		}
	}
}
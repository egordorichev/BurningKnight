using System;
using BurningKnight.state;
using BurningKnight.state.save;
using ImGuiNET;
using Lens;
using Lens.game;
using Lens.graphics.gamerenderer;

namespace BurningKnight.ui.imgui {
	public static class DebugWindow {
		private static string[] states = {
			"ingame", "dialog_editor", "level_editor", "pico", "load", "save_explorer"
		};

		private static Type[] types = {
			typeof(InGameState), typeof(DialogEditorState),
			typeof(EditorState), typeof(PicoState), typeof(LoadState),
			typeof(SaveExplorerState)
		};
		
		public static void Render() {
			if (!ImGui.Begin("Debug")) {
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

			ImGui.DragFloat("Speed", ref Engine.Instance.Speed, 0.05f, 0.1f, 2f);
			
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
			
			ImGui.End();
		}
	}
}
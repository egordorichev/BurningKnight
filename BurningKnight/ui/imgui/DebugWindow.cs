using System;
using System.Collections.Generic;
using System.Diagnostics;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.state.save;
using ImGuiNET;
using Lens;
using Lens.game;
using Lens.graphics.gamerenderer;
using Lens.util.camera;

namespace BurningKnight.ui.imgui {
	public static class DebugWindow {
		private static string[] states = {
			"ingame", "dialog_editor", "level_editor", "pico", "load", "save_explorer", "room_editor"
		};
		
		private static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

		private static Type[] types = {
			typeof(InGameState), typeof(DialogEditorState),
			typeof(EditorState), typeof(PicoState), typeof(LoadState),
			typeof(SaveExplorerState), typeof(RoomEditorState)
		};
		
		private static float[] fps = new float[60];
		private static float[] cpuUsage = new float[60];
		private static int pos;
		private static int lastCall;
		private static float lastFps;

		public static void Render() {
			if (!WindowManager.Debug) {
				return;
			}
			
			if (!ImGui.Begin("Debug", ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				return;
			}
			
			var current = 0;
			var t = Engine.Instance.State.GetType();

			for (var i = 0; i < types.Length; i++) {
				if (t == types[i]) {
					current = i;
					break;
				}
			}
			
			var old = current;

			if (ImGui.Combo("State", ref current, states, states.Length) && old != current) {
				if (current == 0) {
					Engine.Instance.SetState(new LoadState());
				} else {
					Engine.Instance.SetState((GameState) Activator.CreateInstance(types[current]));
				}
			}
			
			if (ImGui.CollapsingHeader("Performance")) {
				ImGui.Text($"FPS: {Engine.Instance.Counter.CurrentFramesPerSecond}");
				lastFps += Engine.Delta;

				if (lastFps > 0.5f) {
					lastFps = 0;

					for (var i = 1; i < fps.Length; i++) {
						fps[i - 1] = fps[i];
					}

					fps[fps.Length - 1] = Engine.Instance.Counter.AverageFramesPerSecond;
				}

				ImGui.PlotHistogram("FPS", ref fps[0], fps.Length, 0, null, 0, 60);

				if (lastCall < (int) Engine.Time) {
					lastCall = (int) Engine.Time;

					for (var i = 1; i < cpuUsage.Length; i++) {
						cpuUsage[i - 1] = cpuUsage[i];
					}

					cpuUsage[cpuUsage.Length - 1] = (float) Math.Round(cpuCounter.NextValue());
				}

				ImGui.PlotHistogram("CPU", ref cpuUsage[0], cpuUsage.Length, 0, null, 0, 100);


				ImGui.DragFloat("Speed", ref Engine.Instance.Speed, 0.01f, 0.1f, 2f);

				ImGui.Text($"Draw calls: {Engine.Graphics.GraphicsDevice.Metrics.DrawCount}");
				ImGui.Text($"Clear calls: {Engine.Graphics.GraphicsDevice.Metrics.ClearCount}");
				ImGui.Text($"FBO binds: {Engine.Graphics.GraphicsDevice.Metrics.TargetCount}");
				ImGui.Text($"Shader binds: {Engine.Graphics.GraphicsDevice.Metrics.PixelShaderCount}");
				ImGui.Text($"Texture count: {Engine.Graphics.GraphicsDevice.Metrics.TextureCount}");
				ImGui.Spacing();
				ImGui.Checkbox("Enable batcher", ref GameRenderer.EnableBatcher);
			}

			if (ImGui.CollapsingHeader("Run info")) {
				ImGui.Text($"Run ID: {GlobalSave.RunId}");
				ImGui.Text($"Seed: {Run.Seed}");
				ImGui.Text($"Kills: {Run.KillCount}");
				ImGui.Text($"Time: {Run.FormatTime()}");
				ImGui.Text($"Has run: {Run.HasRun}");
			}

			if (ImGui.CollapsingHeader("Camera")) {
				var c = Camera.Instance;
				var v = c.X;

				if (ImGui.DragFloat("X", ref v)) {
					c.X = v;
				}
				
				v = c.Y;

				if (ImGui.DragFloat("Y", ref v)) {
					c.Y = v;
				}

				ImGui.Checkbox("Detatched", ref c.Detached);

				v = c.Zoom;

				if (ImGui.InputFloat("Zoom", ref v)) {
					c.Zoom = v;
				}

				ImGui.InputFloat("Texture zoom", ref c.TextureZoom);
			}

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

			ImGui.SameLine();

			if (ImGui.Button("Kill")) {
				LocalPlayer.Locate(Run.Level.Area)?.GetComponent<HealthComponent>().Kill(null);
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
using BurningKnight.assets;
using BurningKnight.save;
using BurningKnight.ui.imgui;
using ImGuiNET;
using Lens.game;
using Lens.graphics;
using Lens.util.file;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BurningKnight.state.save {
	public class SaveExplorerState : GameState {
		public override void Init() {
			base.Init();
			RefreshSaveDir();
		}

		public override void RenderNative() {
			ImGuiHelper.Begin();
			RenderGui();
			ImGuiHelper.End();

			Graphics.Batch.Begin();
			Graphics.Batch.DrawCircle(new CircleF(Mouse.GetState().Position, 3f), 8, Color.White);
			Graphics.Batch.End();
		}

		private FileHandle saveDir;
		private SaveGroup nodes;

		private void RefreshSaveDir() {
			saveDir = new FileHandle(SaveManager.SaveDir);
			nodes = new SaveGroup();

			AddNodes(saveDir, nodes);
		}

		private void AddNodes(FileHandle dir, SaveGroup group) {
			foreach (var d in dir.ListDirectoryHandles()) {
				var node = new SaveGroup {
					FullPath = d.FullPath,
					Name = d.Name
				};
				
				group.Dirs.Add(node);
				AddNodes(d, node);
			}

			foreach (var f in dir.ListFileHandles()) {
				var node = new SaveData {
					FullPath = f.FullPath,
					Name = f.Name
				};
				
				group.Files.Add(node);
				node.Load();
			}
		}

		private void RenderGui() {
			ImGui.Begin("Save Files");
			ImGui.Text($"Save directory: \"{SaveManager.SaveDir}\"");
			
			ImGui.Text($"Exists: {saveDir.Exists()}");
			ImGui.SameLine();

			if (ImGui.Button("Refresh")) {
				RefreshSaveDir();
			}

			nodes.Render();
			ImGui.End();
		}
	}
}
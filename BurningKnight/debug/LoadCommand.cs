using BurningKnight.entity.editor;
using BurningKnight.save;
using BurningKnight.state;
using Lens;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.debug {
	public class LoadCommand : ConsoleCommand {
		public Editor Editor;
		
		public LoadCommand(Editor editor) {
			Name = "load";
			ShortName = "l";
			Editor = editor;
		}
		
		public override void Run(Console console, string[] args) {
			if (args.Length != 1 && args.Length != 2) {
				console.Print("load [save path] (save type)");
				return;
			}

			var path = args[0];
			var saveType = args.Length == 1 ? "all" : args[1];
			var area = Engine.Instance.State.Area;
			
			switch (saveType) {
				case "all": {
					area.Destroy();
					
					Engine.Instance.SetState(new LoadState {
						Path = path
					});
					
					break;
				}

				case "prefab": {
					foreach (var e in Editor.Area.Tags[Tags.LevelSave]) {
						e.Done = true;
					}

					SaveManager.Load(Editor.Area, SaveType.Level, $"{FileHandle.FromRoot("Prefabs/").FullPath}/{path}.lvl");
					Editor.Level = state.Run.Level;
					
					Camera.Instance.Position = Vector2.Zero;
					
					break;
				}
				
				default: {
					console.Print($"Unknown save type {saveType}. Should be one of all, prefab");
					break;
				}
			}
		}
	}
}
using BurningKnight.entity.editor;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.tile;
using Microsoft.Xna.Framework;

namespace BurningKnight.debug {
	public class NewCommand : ConsoleCommand {
		public Editor Editor;
		
		public NewCommand(Editor editor) {
			Name = "new";
			ShortName = "n";
			Editor = editor;
		}
		
		public override void Run(Console Console, string[] Args) {
			int width = 32;
			int height = 32;

			if (Args.Length == 1) {
				width = int.Parse(Args[0]);
				height = width;
			} else if (Args.Length == 2) {
				width = int.Parse(Args[0]);
				height = int.Parse(Args[1]);
			}

			Editor.Area.Destroy();
			Editor.Area.NoInit = true;
			Editor.Area.Add(Editor);
			Editor.Area.NoInit = false;
			
			Editor.Area.Add(Editor.Level = new RegularLevel(BiomeRegistry.Defined[Biome.Castle]) {
				Width = width, Height = height
			});

			Editor.Level.Setup();
			Editor.Level.Fill(Tiles.RandomFloor());
			Editor.Level.TileUp();
			
			Editor.Camera.Position = Vector2.Zero;
		}
	}
}
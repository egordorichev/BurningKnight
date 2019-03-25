using BurningKnight.save;
using Lens;

namespace BurningKnight.debug {
	public class LoadCommand : ConsoleCommand {
		public LoadCommand() {
			Name = "load";
			ShortName = "l";
		}
		
		public override void Run(Console console, string[] args) {
			if (args.Length != 1 && args.Length != 2) {
				console.Print("load [save path] (save type)");
				return;
			}

			var path = args[0];
			var saveType = args.Length == 1 ? "all" : args[1];
			var area = Engine.Instance.State.Area;
			
			// todo: load
			switch (saveType) {
				case "all": {
					SaveManager.Save(area, SaveManager.SaveType.Level, false, path);
					SaveManager.Save(area, SaveManager.SaveType.Player, false, path);
					SaveManager.Save(area, SaveManager.SaveType.Game, false, path);
					SaveManager.Save(area, SaveManager.SaveType.Global, false, path);
					break;
				}

				case "level": {
					SaveManager.Save(area, SaveManager.SaveType.Level, false, path);
					break;
				}

				case "player": {
					SaveManager.Save(area, SaveManager.SaveType.Player, false, path);
					break;
				}

				case "game": {
					SaveManager.Save(area, SaveManager.SaveType.Game, false, path);
					break;
				}

				case "global": {
					SaveManager.Save(area, SaveManager.SaveType.Global, false, path);
					break;
				}

				case "run": {
					SaveManager.Save(area, SaveManager.SaveType.Level, false, path);
					SaveManager.Save(area, SaveManager.SaveType.Player, false, path);
					SaveManager.Save(area, SaveManager.SaveType.Game, false, path);
					break;
				}

				default: {
					console.Print($"Unknown save type ${saveType}. Should be one of all, level, player, game, global, run");
					break;
				}
			}
				
			console.Print($"Done loading {saveType}");
		}
	}
}
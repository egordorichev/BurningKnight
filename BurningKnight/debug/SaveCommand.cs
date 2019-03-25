using System.Threading;
using BurningKnight.save;
using Lens;

namespace BurningKnight.debug {
	public class SaveCommand : ConsoleCommand {
		public SaveCommand() {
			Name = "save";
			ShortName = "s";
		}
		
		public override void Run(Console console, string[] args) {
			if (args.Length != 1 && args.Length != 2) {
				console.Print("save [save path] (save type)");
				return;
			}

			var path = args[0];
			var saveType = args.Length == 1 ? "all" : args[1];
			var area = Engine.Instance.State.Area;
			
			var thread = new Thread(() => {
				switch (saveType) {
					case "all": {
						SaveManager.Save(area, SaveManager.SaveType.Level, path);
						SaveManager.Save(area, SaveManager.SaveType.Player, path);
						SaveManager.Save(area, SaveManager.SaveType.Game, path);
						SaveManager.Save(area, SaveManager.SaveType.Global, path);
						break;
					}

					case "level": {
						SaveManager.Save(area, SaveManager.SaveType.Level, path);
						break;
					}

					case "player": {
						SaveManager.Save(area, SaveManager.SaveType.Player, path);
						break;
					}

					case "game": {
						SaveManager.Save(area, SaveManager.SaveType.Game, path);
						break;
					}

					case "global": {
						SaveManager.Save(area, SaveManager.SaveType.Global, path);
						break;
					}

					case "run": {
						SaveManager.Save(area, SaveManager.SaveType.Level, path);
						SaveManager.Save(area, SaveManager.SaveType.Player, path);
						SaveManager.Save(area, SaveManager.SaveType.Game, path);
						break;
					}

					default: {
						console.Print($"Unknown save type ${saveType}. Should be one of all, level, player, game, global, run");
						break;
					}
				}
					
				console.Print($"Done saving {saveType}");
			});
			
			thread.Start();
		}
	}
}
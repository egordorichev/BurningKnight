using System;
using BurningKnight.ui.editor;
using Lens;
using Lens.entity;
using Lens.util;

namespace BurningKnight.debug {
	public class SpawnCommand : ConsoleCommand {
		public SpawnCommand() {
			Name = "spawn";
			ShortName = "s";
		}
		
		public override void Run(Console Console, string[] Args) {
			if (Args.Length != 1) {
				Console.Print("Usage: spawn [entity]");
				return;
			}

			var name = Args[0];

			foreach (var type in EntityEditor.Types) {
				if (type.Name == name) {
					try {
						var entity = (Entity) Activator.CreateInstance(type.Type);
						Console.GameArea.Add(entity);
						entity.BottomCenter = Console.GameArea.Tagged[Tags.Player][0].BottomCenter;
					} catch (Exception e) {
						Log.Error(e);
						Console.Print($"Failed to create entity {name}, consult @egordorichev");
					}
					
					return;
				}
			}
			
			Console.Print($"Unknown entity {name}");
		}
	}
}
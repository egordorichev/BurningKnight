using Lens.util;
using Lens.util.file;
using MoonSharp.Interpreter;

namespace BurningKnight.assets.mod {
	public class Mod {
		public string Prefix { get; private set; } = "nil";
		private Script script = new Script();
		
		private bool TryCall(string function) {
			var fn = script.Globals[function];
			
			if (fn == null) {
				return false;
			}

			script.Call(fn);
			return true;
		}
		
		public static Mod Load(FileHandle directory) {
			var modFile = directory.FindFile("mod.lua");

			if (!modFile.Exists()) {
				Log.Error($"{modFile.FullPath} was not found!");
				return null;
			}

			var mod = new Mod();

			mod.script.DoFile(modFile.FullPath);
			mod.TryCall("init");
						
			return mod;
		}
	}
}
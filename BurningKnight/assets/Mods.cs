using System;
using System.Collections.Generic;
using System.Reflection;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.assets {
	public static class Mods {
		public static Dictionary<string, object> Loaded = new Dictionary<string, object>();
		
		public static void Load() {
			var dir = new FileHandle("Content/Mods/");

			if (!dir.Exists()) {
				return;
			}
			
			Log.Info("Found mod directory");

			foreach (var handle in dir.ListFileHandles()) {
				if (handle.Extension == ".dll") {
					var DLL = Assembly.LoadFile(handle.FullPath);

					foreach(var type in DLL.GetExportedTypes()) {
						Log.Info($"Found {handle.Name}");
						
						if (type.Name == "Program") {
							Log.Info($"Loading mod {handle.NameWithoutExtension}!");
							
							var c = Activator.CreateInstance(type);
							Loaded[handle.NameWithoutExtension] = c;
							TryInvoke(c, "Init");
						}
					}
				}
			}
		}

		public static bool TryInvoke(object on, string method, params object[] args) {
			try {
				on.GetType().InvokeMember(method, BindingFlags.InvokeMethod, null, on, args);
			} catch (MissingMethodException e) {
				return false;
			} catch (Exception e) {
				Log.Error(e);
			}

			return true;
		}
	}
}
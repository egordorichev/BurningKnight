using System;
using System.Collections.Generic;

namespace Lens.entity {
	public class BitTag {
		public BitTag[] Tags = new BitTag[32];
		public static byte Total;
		
		private static Dictionary<string, BitTag> byName = new Dictionary<string, BitTag>(StringComparer.OrdinalIgnoreCase);
		
		public int ID;
		public int Value;
		public string Name;

		public BitTag(string name) {
			if (Total >= 32) {
				throw new Exception("Can't define more than 32 tags");
			}
			
			if (byName.ContainsKey(name)) {
				throw new Exception("Tag " + name + " is already defined!");
			}
			
			Name = name;
			ID = Total;
			Value = 1 << Total;

			Tags[ID] = this;
			byName[name] = this;

			Total++;
		}

		public static void Define(string name) {
			new BitTag(name);
		}
		
		public static implicit operator int(BitTag tag) {
			return tag.Value;
		}
	}
}
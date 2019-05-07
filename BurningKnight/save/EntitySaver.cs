using System;
using System.Collections.Generic;
using BurningKnight.util;
using Lens.entity;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.save {
	public abstract class EntitySaver : Saver {
		public class Comparer : IComparer<Entity> {
			public int Compare(Entity x, Entity y) {
				return x.GetType().FullName.CompareTo(y.GetType().FullName);
			}
		}

		public static Comparer DefaultComparer;
		
		public void SmartSave(List<Entity> a, FileWriter writer) {
			writer.WriteInt32(a.Count);
			a.Sort(DefaultComparer);

			var all = ArrayUtils.Clone(a);
			
			SaveableEntity last = null;
			
			for (var i = 0; i < all.Length; i++) {
				var entity = (SaveableEntity) all[i];

				if (last != null && last.GetType().FullName == entity.GetType().FullName) {
					writer.WriteString(null);
				} else {
					writer.WriteString(entity.GetType().FullName.Replace("BurningKnight.", ""));
				}

				writer.Cache = true;
				entity.Save(writer);
				writer.Cache = false;

				// todo: write 1 bit (if true second is used, short is written) (to handle big entities like level)
				writer.WriteByte((byte) writer.CacheSize);
				writer.Flush();
				
				last = entity;
			}
		}

		public override void Load(Area area, FileReader reader) {
			var count = reader.ReadInt32();
			var lastType = "";

			for (var i = 0; i < count; i++) {
				var type = reader.ReadString();

				if (type == null) {
					type = lastType;
				}
				
				var entity = (SaveableEntity) Activator.CreateInstance(Type.GetType($"BurningKnight.{type}", true, false));
				area.Add(entity, false);

				var size = reader.ReadByte();
				var position = reader.Position;
				entity.Load(reader);
				var sum = reader.Position - position - size;

				if (sum != 0) {
					Log.Error($"Entity {entity.GetType().FullName} was expected to read {size} bytes but read {reader.Position - position}!");
					reader.Position -= sum;
				}
				
				Log.Error($"Loaded {entity.GetType().FullName}");
				
				entity.PostInit();
				lastType = type;
			}
		}
	}
}
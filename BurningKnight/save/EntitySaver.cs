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

				writer.WriteInt16((short) writer.CacheSize);
				writer.Flush();
				
				last = entity;
			}
		}

		public override void Load(Area area, FileReader reader) {
			Load(area, reader);
		}

		public void Load(Area area, FileReader reader, bool post = true) {
			var count = reader.ReadInt32();
			var lastType = "";

			for (var i = 0; i < count; i++) {
				var type = reader.ReadString();

				if (type == null) {
					type = lastType;
				}
				
				ReadEntity(area, reader, type, post);
				
				lastType = type;
			}
		}

		protected virtual void ReadEntity(Area area, FileReader reader, string type, bool post) {
			var size = reader.ReadInt16();
			var position = reader.Position;

			try {
				var entity = (SaveableEntity) Activator.CreateInstance(Type.GetType($"BurningKnight.{type}", true, false));
				area.Add(entity, false);

				entity.Load(reader);
				var sum = reader.Position - position - size;

				if (sum != 0) {
					Log.Error($"Entity {entity.GetType().FullName} was expected to read {size} bytes but read {reader.Position - position}!");
					reader.Position -= sum;
				}

				if (post) {
					entity.PostInit();
				}
			} catch (Exception e) {
				Log.Error($"Failed to load {type}");
				Log.Error(e);

				reader.Position = position + size;
			}
		}

		protected EntitySaver(SaveType type) : base(type) {
			
		}
	}
}
using System;
using System.Collections.Generic;
using BurningKnight.level;
using Lens.entity;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.save {
	public class PrefabSaver : LevelSave {
		public List<PrefabData> Datas = new List<PrefabData>();

		public override void Load(Area area, FileReader reader) {
			Datas.Clear();
			base.Load(area, reader);
		}

		protected override void ReadEntity(Area area, FileReader reader, string type, bool post) {
			if (type == "level.rooms.Room") {
				type = "entity.room.Room";
			} else if (type == "entity.item.ItemStand") {
				type = "entity.item.stand.ItemStand";
			}
		
			var t = Type.GetType($"BurningKnight.{type}", true, false);

			if (typeof(Level).IsAssignableFrom(t)) {
				base.ReadEntity(area, reader, type, post);
				return;
			}
			
			var prefab = new PrefabData();
			prefab.Type = t;

			var size = reader.ReadInt16();
			prefab.Data = new byte[size];
			
			for (var i = 0; i < size; i++) {
				prefab.Data[i] = reader.ReadByte();
			}
			
			Datas.Add(prefab);
		}
	}
}
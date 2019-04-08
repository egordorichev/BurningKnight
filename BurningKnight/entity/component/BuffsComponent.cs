using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.entity.buff;
using BurningKnight.entity.events;
using BurningKnight.entity.level;
using BurningKnight.entity.level.tile;
using Lens.entity;
using Lens.entity.component;
using Lens.util.file;

namespace BurningKnight.entity.component {
	public class BuffsComponent : SaveableComponent {
		public Dictionary<Type, Buff> Buffs = new Dictionary<Type, Buff>();
		public List<Type> Immune = new List<Type>();

		public void Add<T>() {
			var type = typeof(T);
			
			if (Buffs.ContainsKey(type)) {
				return;
			}
			
			foreach (var t in Immune) {
				if (t == type) {
					return;
				}
			}

			var buff = (Buff) Activator.CreateInstance(type);
			Buffs[type] = buff;
			
			Send(new BuffAddedEvent {
				Buff = buff
			});
		}

		public void Add(Buff buff) {
			if (buff == null) {
				return;
			}
			
			var type = buff.GetType();
			
			if (Buffs.ContainsKey(type)) {
				return;
			}
			
			foreach (var t in Immune) {
				if (t == type) {
					return;
				}
			}

			Buffs[type] = buff;
			
			Send(new BuffAddedEvent {
				Buff = buff
			});
		}

		public void Add(string id) {
			Add(BuffRegistry.Create(id));
		}

		public bool Has<T>() {
			return Buffs.ContainsKey(typeof(T));
		}

		public void Remove<T>() {
			var type = typeof(T);

			if (Buffs.TryGetValue(type, out var buff)) {
				Send(new BuffRemovedEvent {
					Buff = buff
				});
				
				buff.Destroy();
				Buffs.Remove(type);
			}
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			foreach (var buff in Buffs.Values) {				
				buff.Update(dt);
			}

			foreach (var key in Buffs.Keys.ToList()) {
				var buff = Buffs[key];

				if (buff.Duration <= 0) {
					buff.Destroy();
					Buffs.Remove(key);
				}
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte((byte) Buffs.Count);
			
			foreach (var buff in Buffs) {
				stream.WriteString(buff.Value.Id);
			}
		}

		public override void Load(FileReader reader) {
			base.Load(reader);
			var count = reader.ReadByte();

			for (int i = 0; i < count; i++) {
				Add(reader.ReadString());
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is TileCollisionStartEvent tileStart) {
				if (tileStart.Tile == Tile.Water) {
					Remove<BurningBuff>();
				}
			} else if (e is FlagCollisionStartEvent flagStart) {
				if (flagStart.Flag == Flag.Burning) {
					Add<BurningBuff>();
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}
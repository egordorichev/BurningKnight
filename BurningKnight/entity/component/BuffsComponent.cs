using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.entity.buff;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class BuffsComponent : Component {
		public Dictionary<Type, Buff> Buffs = new Dictionary<Type, Buff>();
		public List<Type> Immune = new List<Type>();

		public void Add<T>() {
			var type = typeof(T);
			
			foreach (var t in Immune) {
				if (t == type) {
					return;
				}
			}
			
			if (Buffs.ContainsKey(type)) {
				return;
			}

			Buffs[type] = (Buff) Activator.CreateInstance(type);
		}

		public bool Has<T>() {
			return Buffs.ContainsKey(typeof(T));
		}

		public void Remove<T>() {
			var type = typeof(T);

			if (Buffs.TryGetValue(type, out var buff)) {
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
	}
}
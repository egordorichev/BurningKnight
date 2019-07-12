using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.entity.buff;
using BurningKnight.entity.events;
using BurningKnight.level;
using BurningKnight.level.tile;
using ImGuiNET;
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
			buff.Init();
			
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

				if (buff.TimeLeft <= 0) {
					buff.Destroy();
					Buffs.Remove(key);
				}
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte((byte) Buffs.Count);
			
			foreach (var buff in Buffs) {
				stream.WriteString(buff.Value.Type);
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
			foreach (var b in Buffs.Values) {
				b.HandleEvent(e);
			}
			
			if (e is TileCollisionStartEvent tileStart) {
				if (tileStart.Tile == Tile.Water) {
					Remove<BurningBuff>();
				}
			}/* else if (e is FlagCollisionStartEvent flagStart) {
				if (flagStart.Flag == Flag.Burning) {
					Add<BurningBuff>();
				}
			}*/
			
			return base.HandleEvent(e);
		}

		private static string toAdd = "";

		public override void RenderDebug() {
			if (ImGui.InputText("Buff", ref toAdd, 128)) {
				Add(toAdd);
				toAdd = "";
			}
			
			ImGui.SameLine();
				
			if (ImGui.Button("Add")) {
				Add(toAdd);
				toAdd = "";
			}

			if (Buffs.Count == 0) {
				ImGui.BulletText("No buffs");
				return;
			}
			
			foreach (var b in Buffs.Values) {
				if (toAdd.Length > 0 && !BuffRegistry.All.ContainsKey(toAdd)) {
					ImGui.BulletText("Unknown buff");
				}
				
				if (ImGui.TreeNode($"{b.Type}")) {
					ImGui.Text($"{b.TimeLeft} seconds left");
					
					if (ImGui.Button($"Remove##{b.Type}")) {
						b.TimeLeft = 0;
					}
					
					ImGui.SameLine();
					
					if (ImGui.Button($"Renew##{b.Type}")) {
						b.TimeLeft = b.Duration;
					}

					ImGui.Checkbox($"Infinite##{b.Type}", ref b.Infinite);
					ImGui.TreePop();
				}
			}
		}
	}
}
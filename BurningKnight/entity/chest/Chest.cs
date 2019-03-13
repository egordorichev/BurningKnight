using System;
using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.save;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.entity.chest {
	public class Chest : SaveableEntity {
		public bool IsOpen { get; private set; }
		protected List<Item> items = new List<Item>();
		
		public void Open() {
			if (IsOpen) {
				return;
			}

			IsOpen = true;
		}

		public virtual void GenerateLoot() {
			items.Add(ItemRegistry.Create("health_potion"));
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			IsOpen = stream.ReadBoolean();

			if (!IsOpen) {
				var count = stream.ReadByte();

				for (int i = 0; i < count; i++) {
					items.Add(ItemRegistry.Create(stream.ReadString()));
				}
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(IsOpen);

			if (!IsOpen) {
				stream.WriteByte((byte) items.Count);

				foreach (var item in items) {
					stream.WriteString(item.Id);
				}
			}
		}

		protected bool Interact(Entity entity) {
			Open();
			return true;
		}

		protected virtual bool CanInteract() {
			return true;
		}
		
		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new InteractableComponent(Interact) {
				CanInteract = CanInteract
			});			
		}
	}
}
using System;
using System.Collections.Generic;
using System.Text;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.use;
using BurningKnight.entity.item.useCheck;
using BurningKnight.save;
using Lens.assets;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.util.file;

namespace BurningKnight.entity.item {
	public class Item : SaveableEntity {
		private int count = 1;

		public int Count {
			get => count;
			set {
				count = Math.Max(0, value);

				if (count == 0) {
					Done = true;
				}
			}
		}

		public string Id;
		public string Name => Locale.Get(Id);
		public string Description => Locale.Get($"{Id}_desc");
		public float UseTime = 0.3f;
		public float Delay { get; protected set; }
		
		public ItemUse[] Uses;
		public ItemUseCheck UseCheck = ItemUseChecks.Default;
		
		public Item(params ItemUse[] uses) {
			Uses = uses;
		}

		public void Use(Player player) {
			if (!UseCheck.CanUse(player, this)) {
				return;
			}

			foreach (var use in Uses) {
				use.Use(player, this);
			}
		}

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new ImageComponent(Id));
		}

		public override void Save(FileWriter stream) {
			stream.WriteInt32(Count);
			stream.WriteString(Id);
		}

		public override void Load(FileReader stream) {
			Count = stream.ReadInt32();
			Id = stream.ReadString();
			
			
		}

		public StringBuilder BuildInfo() {
			var builder = new StringBuilder();
			builder.Append(Name).Append('\n').Append(Description);
			
			return builder;
		}
	}
}
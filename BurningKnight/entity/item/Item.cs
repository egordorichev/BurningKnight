using System;
using System.Text;
using Lens.assets;
using Lens.entity.component.graphics;
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
		
		public bool Stackable = false;
		public bool Usable = true;

		public string Id { get; private set; }
		public string Name => Locale.Get(Id);
		public string Description => Locale.Get($"{Id}_desc");

		public Item() {
			Id = GetType().Name;
			// TODO: id to pascal_case
		}

		protected override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ImageComponent(Id));
		}

		public override void Save(FileWriter stream) {
			stream.WriteInt32(Count);
		}

		public override void Load(FileReader stream) {
			Count = stream.ReadInt32();
		}

		public StringBuilder BuildInfo() {
			var builder = new StringBuilder();
			builder.Append(Name).Append('\n').Append(Description);
			
			return builder;
		}
	}
}
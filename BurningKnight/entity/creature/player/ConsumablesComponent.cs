using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.util;
using Lens.input;
using Lens.util.file;

namespace BurningKnight.entity.creature.player {
	public class ConsumablesComponent : ItemComponent {
		private byte bombs;
		private byte keys;
		private byte coins;

		public int Bombs {
			set => bombs = (byte) MathUtils.Clamp(0, 99, value);
			get => bombs;
		}
		
		public int Keys {
			set => keys = (byte) MathUtils.Clamp(0, 99, value);
			get => keys;
		}
		
		public int Coins {
			set => coins = (byte) MathUtils.Clamp(0, 99, value);
			get => coins;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (bombs > 0 && Input.WasPressed(Controls.Bomb)) {
				Bombs--;
				// fixme: spawn a bomb
			}
		}

		public override void Set(Item item) {
			if (item.Type == ItemType.Bomb) {
				Bombs += item.Count;
			} else if (item.Type == ItemType.Key) {
				Keys += item.Count;
			} else if (item.Type == ItemType.Coin) {
				Coins += item.Count;
			}
			
			item.Done = true;
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Bomb || item.Type == ItemType.Key || item.Type == ItemType.Coin;
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte(bombs);
			stream.WriteByte(keys);
			stream.WriteByte(coins);
		}

		public override void Load(FileReader reader) {
			base.Load(reader);

			bombs = reader.ReadByte();
			keys = reader.ReadByte();
			coins = reader.ReadByte();
		}
	}
}
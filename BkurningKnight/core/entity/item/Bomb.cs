using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.entity;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item {
	public class Bomb : Item {
		protected void _Init() {
			{
				Name = Locale.Get("bomb");
				Description = Locale.Get("bomb_desc");
				Sprite = "item-bomb";
				UseTime = 0.3f;
				Stackable = true;
				AutoPickup = true;
			}
		}

		public override Void Generate() {
			base.Generate();
			SetCount(Random.NewInt(3, 8));
		}

		public override Void Use() {
			if (this.Delay > 0) {
				return;
			} 

			base.Use();
			this.Count -= 1;
			BombEntity E = new BombEntity(this.Owner.X + (this.Owner.W - 16) / 2, this.Owner.Y + (this.Owner.H - 16) / 2).ToMouseVel();
			E.Owner = this.Owner;

			if (this.Owner is Player) {
				Player Player = (Player) this.Owner;
				E.LeaveSmall = Player.LeaveSmall;
			} 

			Dungeon.Area.Add(E);
		}

		public Bomb() {
			_Init();
		}
	}
}

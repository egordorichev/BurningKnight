using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.entity;
using BurningKnight.util;

namespace BurningKnight.entity.item {
	public class Bomb : Item {
		public Bomb() {
			_Init();
		}

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

		public override void Generate() {
			base.Generate();
			SetCount(Random.NewInt(3, 8));
		}

		public override void Use() {
			if (Delay > 0) return;

			base.Use();
			Count -= 1;
			var E = new BombEntity(Owner.X + (Owner.W - 16) / 2, Owner.Y + (Owner.H - 16) / 2).ToMouseVel();
			E.Owner = Owner;

			if (Owner is Player) {
				var Player = (Player) Owner;
				E.LeaveSmall = Player.LeaveSmall;
			}

			Dungeon.Area.Add(E);
		}
	}
}
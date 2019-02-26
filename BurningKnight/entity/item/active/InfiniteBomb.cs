using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.entity;

namespace BurningKnight.entity.item.active {
	public class InfiniteBomb : ActiveItem {
		public InfiniteBomb() {
			_Init();
		}

		protected void _Init() {
			{
				Sprite = "item-bomb_orbital";
				Stackable = false;
				UseTime = 0.4f;
				Name = Locale.Get("infinite_bomb");
				Description = Locale.Get("infinite_bomb_desc");
			}
		}

		public override void Use() {
			if (Delay > 0) return;

			base.Use();
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
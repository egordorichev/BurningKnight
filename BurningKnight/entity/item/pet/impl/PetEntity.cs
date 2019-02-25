using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities;
using BurningKnight.util;

namespace BurningKnight.entity.item.pet.impl {
	public class PetEntity : SaveableEntity {
		protected bool NoTp;

		public Player Owner;
		public TextureRegion Region = Item.Missing;
		protected string Sprite = "";
		public float Z;

		public PetEntity() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (!OnScreen && !NoTp) {
				this.X = Owner.X + Owner.W / 2;
				this.Y = Owner.Y + Owner.H / 2;
				Tp();
			}
		}

		protected void Tp() {
		}

		public override void Init() {
			base.Init();

			if (!Sprite.IsEmpty()) Region = Graphics.GetTexture(Sprite);

			Owner = Player.Instance;
			double A = Random.NewFloat() * Math.PI * 2;
			var D = 24f;
			this.X = Owner.X + Owner.W / 2 + Math.Cos(A) * D;
			this.Y = Owner.Y + Owner.H / 2 + Math.Sin(A) * D;
		}

		public override void Render() {
			Graphics.Render(Region, this.X, this.Y);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Creature || Entity is Door || Entity is SolidProp || Entity is ItemHolder && !(((ItemHolder) Entity).GetItem() is Gold)) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}
	}
}
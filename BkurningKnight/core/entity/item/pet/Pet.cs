using BurningKnight.core.entity.item.autouse;
using BurningKnight.core.entity.item.pet.impl;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.pet {
	public class Pet : Autouse {
		public PetEntity Create() {
			return new PetEntity();
		}

		public override Void Generate() {
			SetCount(Random.NewInt(1, 4));
		}

		public override Void Use() {
			base.Use();
			PetEntity Entity = Create();
			double A = Random.NewFloat() * Math.PI * 2;
			float D = 24f;
			Entity.X = this.Owner.X + this.Owner.W / 2 + (float) (Math.Cos(A) * D);
			Entity.Y = this.Owner.Y + this.Owner.H / 2 + (float) (Math.Sin(A) * D);
			Dungeon.Area.Add(Entity);
			PlayerSave.Add(Entity);
		}
	}
}

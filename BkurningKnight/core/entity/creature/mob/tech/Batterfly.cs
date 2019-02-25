using BurningKnight.core.entity.creature.mob.common;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.fx;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.tech {
	public class Batterfly : DiagonalFly {
		public static Animation Animations = Animation.Make("actor-fly", "-electro");
		private Dictionary<Mob, Lighting> Lighting = new Dictionary<>();

		public Animation GetAnimation() {
			return Animations;
		}

		protected override Void CreateBody() {
			W = 19;
			H = 17;
			Body = World.CreateSimpleBody(this, 5, 2, W - 12, H - 4, BodyDef.BodyType.DynamicBody, false);
			Body.GetFixtureList().Get(0).SetRestitution(1f);
			Body.SetTransform(X, Y, 0);
			float F = 32;
			this.Velocity = new Point(F * (Random.Chance(50) ? -1 : 1), F * (Random.Chance(50) ? -1 : 1));
			Body.SetLinearVelocity(this.Velocity);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (Player.Instance.Room == Room) {
				foreach (Mob Mob in Mob.All) {
					if (Mob != this && Mob.Room == Room && !this.Lighting.ContainsKey(Mob)) {
						if (Mob is Batterfly && ((Batterfly) Mob).Lighting.ContainsKey(this)) {
							continue;
						} 

						if (this.GetDistanceTo(Mob.X + Mob.W / 2, Mob.Y + Mob.H / 2) < 96) {
							Lighting Lighting = new Lighting();
							Lighting.X = this.X + W / 2;
							Lighting.Y = this.Y + H / 2;
							Lighting.Bad = true;
							Dungeon.Area.Add(Lighting);
							this.Lighting.Put(Mob, Lighting);
						} 
					} 
				}
			} 

			Iterator<Map.Entry<Mob, Lighting>> It = Lighting.EntrySet().Iterator();

			while (It.HasNext()) {
				Map.Entry<Mob, Lighting> Pair = It.Next();
				Lighting Lighting = Pair.GetValue();
				Mob Mob = Pair.GetKey();
				Lighting.Target.X = Mob.X + Mob.W / 2;
				Lighting.Target.Y = Mob.Y + Mob.H / 2;
				Lighting.W = this.GetDistanceTo(Mob.X + W / 2, Mob.Y + H / 2);
				Lighting.An = (float) (this.GetAngleTo(Mob.X + W / 2, Mob.Y + H / 2) - Math.PI / 2);
				Lighting.A = (float) Math.ToDegrees(Lighting.An);
				Lighting.X = this.X + W / 2;
				Lighting.Y = this.Y + H / 2;
				Lighting.UpdatePos();

				if (Mob.Done || this.GetDistanceTo(Mob.X + W / 2, Mob.Y + H / 2) > 96) {
					Lighting.Done = true;
					It.Remove();
					this.Lighting.Remove(Mob);
				} 
			}
		}

		public override Void Destroy() {
			base.Destroy();

			foreach (Lighting Lighting in Lighting.Values()) {
				Lighting.Done = true;
			}
		}
	}
}

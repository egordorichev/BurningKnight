using BurningKnight.entity.creature.mob;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.entity.pattern;
using BurningKnight.util;

namespace BurningKnight.entity.level.entities.chest {
	public class Mimic : Mob {
		protected void _Init() {
			{
				HpMax = 30;
				H = 13;
				W = 18;
				KnockbackMod = 0;
			}

			{
				IgnoreRooms = true;
			}
		}

		public class MimicState : State<Mimic> {
		}

		public class IdleState : MimicState {
		}

		public class FoundState : MimicState {
			public override void OnEnter() {
				base.OnEnter();
				ReadAnim();
				Animation = Open;
				Self.PlaySfx("chest_open");
				Animation.SetBack(false);
				Animation.SetPaused(false);
				Animation.SetFrame(0);
			}
		}

		public class AttackState : MimicState {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= 3f) Self.Become("close");
			}
		}

		public class CloseState : MimicState {
			public override void OnEnter() {
				base.OnEnter();
				Animation = Open;
				Open.SetBack(true);
				Open.SetPaused(false);
				Open.SetFrame(4);
			}
		}

		public class WaitState : MimicState {
			private int Num;

			public override void OnEnter() {
				base.OnEnter();
				Animation = Closed;
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= (NumAttack % 2 == 1 ? 0.5f : 2f)) {
					if (Num == (NumAttack % 2 == 1 ? 6 : 3)) {
						NumAttack++;
						Self.Become("found");

						return;
					}

					Tween.To(new Tween.Task(0.5f, 0.1f) {

			public override float GetValue() {
				return Sy;
			}

			public override void SetValue(float Value) {
				Sy = Value;
			}

			public override void OnEnd() {
				if (NumAttack % 2 == 1) {
					BulletProjectile Bullet = new NanoBullet();
					Bullet.Bad = true;
					Bullet.Owner = Self;
					Bullet.X = Self.X + Self.W / 2;
					Bullet.Y = Self.Y + Self.H / 2;
					Self.PlaySfx("gun_machinegun");
					var A = Self.GetAngleTo(Self.Target.X + Self.Target.W / 2, Self.Target.Y + Self.Target.H / 2);
					var D = 60f;
					Bullet.Velocity.X = Math.Cos(A) * D;
					Bullet.Velocity.Y = Math.Sin(A) * D;
					Dungeon.Area.Add(Bullet);
				}
				else {
					var Pattern = new CircleBulletPattern();
					Self.PlaySfx("gun_machinegun");
					Pattern.Radius = 8f;

					for (var I = 0; I < 5; I++) Pattern.AddBullet(NewProjectile());

					BulletPattern.Fire(Pattern, Self.X + 10, Self.Y + 8, Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8), 40f);
				}


				Tween.To(new Tween.Task(1f, 0.1f) {

			public override float GetValue() {
				return Sy;
			}

			public override void SetValue(float Value) {
				Sy = Value;
			}

			public override void OnEnd() {
			}
		});
	}
});

Tween.To(new Tween.Task(1.2f, 0.2f) {
public override float GetValue() {
	return Sx;
}

public override void SetValue(float Value) {
	Sx = Value;
}

public override void OnEnd() {
	Tween.To(new Tween.Task(1f, 0.3f) {

public override float GetValue() {
	return Sx;
}

public override void SetValue(float Value) {
	Sx = Value;
}
});
}
});
T = 0;
Num++;
}
if (this.T >= 3f) {
Self.Become("found");
}
}
}
public static float Chance = 10;
public static List<Mimic> All = new List<>();
private AnimationData Closed;
private AnimationData Open;
private AnimationData Hurt;
private AnimationData Animation;
private bool Found;
public bool Weapon;
public bool Locked = true;
private int Type = -1;
private int NumAttack;
public override void Init() {
base.Init();
All.Add(this);
this.Body = World.CreateSimpleBody(this, 1, 0, 14, 13, BodyDef.BodyType.DynamicBody, true);
World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
}
protected override void DeathEffects() {
base.DeathEffects();
this.PlaySfx("death_clown");
this.Done = true;
Dungeon.Area.Add(new Explosion(this.X + this.W / 2, this.Y + this.H / 2));
Dungeon.Area.Add(new Smoke(this.X + this.W / 2, this.Y + this.H / 2));
this.PlaySfx("explosion");
Chest Chest = null;
if (this.Type == 1) {
Chest = new IronChest();
} else if (Type == 2) {
Chest = new GoldenChest();
} else {
Chest = new WoodenChest();
}
Chest.X = this.X;
Chest.Y = this.Y;
Chest.Weapon = Weapon;
Chest.Locked = false;
Chest.SetItem(Chest.Generate());
Dungeon.Area.Add(Chest);
LevelSave.Add(Chest);
Chest.Open();
Achievements.Unlock(Achievements.UNLOCK_MIMIC_TOTEM);
Achievements.Unlock(Achievements.UNLOCK_MIMIC_SUMMONER);
}
public void ToChest() {
Chest Chest;
if (this.Type == 1) {
Chest = new IronChest();
} else if (Type == 2) {
Chest = new GoldenChest();
} else {
Chest = new WoodenChest();
}
Chest.X = this.X;
Chest.Y = this.Y;
Chest.Locked = this.Locked;
Chest.Weapon = this.Weapon;
Chest.SetItem(Chest.Generate());
Dungeon.Area.Add(Chest);
LevelSave.Add(Chest);
this.Done = true;
}
public override void Destroy() {
base.Destroy();
All.Remove(this);
this.Body = World.RemoveBody(this.Body);
}
public override void Load(FileReader Reader) {
base.Load(Reader);
Found = Reader.ReadBoolean();
this.Type = Reader.ReadByte();
Weapon = Reader.ReadBoolean();
Locked = Reader.ReadBoolean();
if (Found) {
this.Become("found");
}
}
public override void Save(FileWriter Writer) {
base.Save(Writer);
Writer.WriteBoolean(Found);
Writer.WriteByte((byte) this.Type);
Writer.WriteBoolean(Weapon);
Writer.WriteBoolean(Locked);
}
private void ReadAnim() {
if (this.Animation == null) {
Animation Animations = WoodenChest.Animation;
if (this.Type == -1) {
Chest Chest = Chest.Random();
if (Chest is IronChest) {
this.Type = 1;
} else if (Chest is GoldenChest) {
this.Type = 2;
} else if (Chest is WoodenChest) {
this.Type = 0;
}
}
if (this.Type == 1) {
Animations = IronChest.Animation;
} else if (Type == 2) {
Animations = GoldenChest.Animation;
}
Closed = Animations.Get("idle");
Open = Animations.Get("opening_mimic");
Open.SetAutoPause(true);
Hurt = Animations.Get("hurt");
this.Animation = this.Closed;
}
}
public override void Render() {
ReadAnim();
Graphics.Batch.SetColor(1, 1, 1, this.A);
this.RenderWithOutline((this.Invt > 0 && this.Animation == Open) ? Hurt : this.Animation);
if (!this.Found) {
float X = this.X + (W - Chest.IdleLock.GetRegionWidth()) / 2;
float Y = this.Y + (H - Chest.IdleLock.GetRegionHeight()) / 2 + (float) Math.Sin(this.T) * 1.8f;
Graphics.Render(Chest.IdleLock, X, Y);
}
RenderStats();
}
public override void KnockBackFrom(Entity From, float Force) {
}
public override void Update(float Dt) {
this.Velocity.X = 0;
this.Velocity.Y = 0;
base.Update(Dt);
foreach (Player Player in Colliding) {
Player.ModifyHp(-1, this, true);
}
this.Saw = true;
if (this.Freezed) {
return;
}
if (Math.Abs(this.Velocity.X) > 1f) {
this.Flipped = this.Velocity.X < 0;
}
if (this.Animation != null) {
if (this.Animation.Update(Dt * SpeedMod)) {
if (this.Animation == Open) {
this.Become(this.Animation.IsGoingBack() ? "wait" : "attack");
}
}
}
if (this.Hurt != null) {
Hurt.Update(Dt);
}
if (this.Dead) {
base.Common();
return;
}
base.Common();
}
public override void OnCollision(Entity Entity) {
base.OnCollision(Entity);
if (Entity is Player) {
if (!this.Found) {
Achievements.Unlock(Achievements.FIND_MIMIC);
this.Found = true;
this.Become("found");
}
}
}
protected override void OnHurt(int A, Entity From) {
base.OnHurt(A, From);
this.PlaySfx("damage_clown");
if (!this.Found) {
Achievements.Unlock(Achievements.FIND_MIMIC);
this.Found = true;
this.Become("found");
}
}
protected override State GetAiWithLow(string State) {
return GetAi(State);
}
protected override State GetAi(string State) {
switch (State) {
case "chase":
case "idle":
case "roam": {
return new IdleState();
}
case "alerted":
case "found": {
return new FoundState();
}
case "attack": {
return new AttackState();
}
case "close": {
return new CloseState();
}
case "wait": {
return new WaitState();
}
}
return base.GetAi(State);
}
protected override bool CanHaveBuff(Buff Buff) {
return !(Buff is PoisonedBuff) && base.CanHaveBuff(Buff);
}
protected override List GetDrops<Item> () {
List<Item> Drops = base.GetDrops();
for (int I = 0; I < Random.NewInt(3, 8); I++) {
ItemHolder Item = new ItemHolder(new Gold());
Item.GetItem().Generate();
Dungeon.Area.Add(Item);
LevelSave.Add(Item);
}
return Drops;
}
public BulletProjectile NewProjectile() {
BulletProjectile Bullet = new NanoBullet();
Bullet.Damage = 1;
Bullet.Owner = this;
Bullet.Bad = true;
float A = 0;
Bullet.X = X;
Bullet.Y = Y;
Bullet.Velocity.X = (float) (Math.Cos(A));
Bullet.Velocity.Y = (float) (Math.Sin(A));
return Bullet;
}
public override bool RollBlock() {
if (!this.Found) {
this.Found = true;
this.Become("found");
this.PlaySfx("damage_clown");
}
return !this.State.Equals("attack") || base.RollBlock();
}
public Mimic() {
_Init();
}
}
}
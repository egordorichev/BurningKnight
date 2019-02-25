using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.desert {
	public class Skeleton : Mob {
		protected void _Init() {
		{
			HpMax = 3;
			W = 13;
			Idle = GetAnimation().Get("idle").Randomize();
			Run = GetAnimation().Get("run").Randomize();
			Hurt = GetAnimation().Get("hurt").Randomize();
			Killed = GetAnimation().Get("death");
			Killed.SetAutoPause(true);
			Revive = GetAnimation().Get("revive");
			Revive.SetListener(new AnimationData.Listener {

		public override void OnEnd() {
			Become("idle");
		}
	});

	Animation = this.Idle;
}

}
public class SkeletonState : Mob.State<Skeleton> {
}
public class AttackState : SkeletonState {
public override void OnEnter() {
base.OnEnter();
Self.Weapon.Use();
}
public override void Update(float Dt) {
base.Update(Dt);
if (T >= 1f) {
JustAttacked = true;
Self.Become("chase");
}
}
}
public class IdleState : SkeletonState {
public override void Update(float Dt) {
base.Update(Dt);
CheckForPlayer();
}
}
public class AlertedState : SkeletonState {
public override void Update(float Dt) {
base.Update(Dt);
if (this.T >= 0.75f) {
Self.Become("chase");
}
}
}
public class ChaseState : SkeletonState {
private Point To;
private bool CanSee;
public override void Update(float Dt) {
base.Update(Dt);
if (this.T < 0.6f) {
if (To == null) {
CanSee = Self.CanSee(Player.Instance, Terrain.HOLE);
float D = Self.GetDistanceTo(Player.Instance.X, Player.Instance.Y);
if (D < 24f && !JustAttacked) {
Self.Become("attack");
return;
}
JustAttacked = false;
if (CanSee) {
float F = 120f;
float A;
if (D < 64) {
if (Random.Chance(50)) {
F = 50f;
A = Self.GetAngleTo(Player.Instance.X, Player.Instance.Y);
} else {
A = Random.NewFloat((float) (Math.PI * 2));
}
} else {
A = Self.GetAngleTo(Player.Instance.X, Player.Instance.Y) + Random.NewFloat(-1f, 1f);
}
To = new Point((float) Math.Cos(A) * F, (float) Math.Sin(A) * F);
} else {
To = new Point(Player.Instance.X, Player.Instance.Y);
}
}
if (CanSee) {
if (T < 0.4f) {
Self.Acceleration.X = To.X;
Self.Acceleration.Y = To.Y;
}
} else {
if (MoveTo(To, 80f, 32f) || Self.CanSee(Player.Instance)) {
To = null;
}
}
} else {
Self.Velocity.Mul(0);
To = null;
if (this.T >= 0.61f) {
this.T = 0;
}
}
}
}
public static Animation Animations = Animation.Make("actor-skeleton", "-white");
private AnimationData Idle;
private AnimationData Run;
private AnimationData Hurt;
private AnimationData Killed;
private AnimationData Revive;
private AnimationData Animation;
public float Distance = 48;
private bool JustAttacked;
private Item Weapon;
public int Side;
public bool Eight;
public float BoneSpeed = 120f;
public Animation GetAnimation() {
return Animations;
}
public override void Init() {
base.Init();
this.Body = this.CreateSimpleBody(2, 1, 12, 12, BodyDef.BodyType.DynamicBody, false);
World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
this.Weapon = new Bone();
this.Weapon.SetOwner(this);
}
public override void Render() {
if (Math.Abs(this.Velocity.X) > 1f) {
this.Flipped = this.Velocity.X < 0;
}
float V = Math.Abs(this.Acceleration.X) + Math.Abs(this.Acceleration.Y);
if (this.State.Equals("revive")) {
this.Animation = Revive;
} else if (this.State.Equals("dead") || this.State.Equals("kindadead")) {
this.Animation = Killed;
} else if (this.Invt > 0) {
this.Animation = Hurt;
} else if (V > 1f) {
this.Animation = Run;
} else {
this.Animation = Idle;
}
this.RenderWithOutline(this.Animation);
Graphics.Batch.SetColor(1, 1, 1, this.A);
this.Weapon.Render(this.X, this.Y, this.W, this.H, this.Flipped, false);
base.RenderStats();
}
protected override List GetDrops<Item> () {
List<Item> Items = base.GetDrops();
if (Random.Chance(5)) {
Items.Add(new Bone());
}
return Items;
}
protected override State GetAi(string State) {
switch (State) {
case "idle":
case "roam": {
return new IdleState();
}
case "alerted": {
return new AlertedState();
}
case "chase": {
return new ChaseState();
}
case "attack": {
return new AttackState();
}
}
return base.GetAi(State);
}
protected override void DeathEffects() {
base.DeathEffects();
this.PlaySfx("death_clown");
this.Done = true;
DeathEffect(Killed);
}
public void Mod(Point Vel, Point Ivel, float A, float D, float Time) {
float V = (float) Math.Cos(Time * 2f);
Vel.X = Ivel.X * V;
Vel.Y = Ivel.Y * V;
}
public override void Update(float Dt) {
if (this.Animation != null) {
this.Animation.Update(Dt * SpeedMod);
}
this.Weapon.Update(Dt * SpeedMod);
base.Update(Dt);
if (this.Freezed) {
return;
}
if (this.Dead) {
base.Common();
return;
}
base.Common();
}
public override void Destroy() {
base.Destroy();
Weapon.Destroy();
}
public static Skeleton Random() {
return new Skeleton();
}
public Skeleton() {
_Init();
}
}
}
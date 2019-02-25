using BurningKnight.entity.creature;
using BurningKnight.util;

namespace BurningKnight.entity.level.entities {
	public class Tree : SolidProp {
		private float Am = 1f;
		public bool Burning;
		private float Damage;
		private float LastFlame;

		protected void _Init() {
			{
				Sprite = "props-tree";
				Collider = new Rectangle(4, 8, 30 - 4 * 2, 30 - 8 * 2);
				W = 30;
				H = 30;
			}
		}

		public override void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Creature) Tween.To(new Tween.Task(4, 0.2f) {

		public override float GetValue() {
			return Am;
		}

		public override void SetValue(float Value) {
			Am = Value;
		}
	});
}

}
public override void Update(float Dt) {
base.Update(Dt);
Am = Math.Max(1, Am - Dt * 2f);
if (!Burning) {
int I = Level.ToIndex((int) Math.Floor((this.X + 15) / 16), (int) Math.Floor((this.Y) / 16));
int Info = Dungeon.Level.GetInfo(I);
if (BitHelper.IsBitSet(Info, 0)) {
this.Damage = 0;
this.Burning = true;
foreach (int J in PathFinder.NEIGHBOURS4) {
Dungeon.Level.SetOnFire(I + J, true);
}
}
} else {
InGameState.Burning = true;
Dungeon.Level.SetOnFire(Level.ToIndex((int) Math.Floor((this.X + 15) / 16), (int) Math.Floor((this.Y) / 16)), true);
this.Damage += Dt * 0.8f;
LastFlame += Dt;
if (this.LastFlame >= 0.05f) {
this.LastFlame = 0;
TerrainFlameFx Fx = new TerrainFlameFx();
Fx.X = this.X + Random.NewFloat(this.W);
Fx.Y = this.Y + Random.NewFloat(this.H) - 4;
Fx.Depth = 1;
Dungeon.Area.Add(Fx);
}
if (this.Damage >= 1f) {
this.Done = true;
for (int I = 0; I < 10; I++) {
PoofFx Fx = new PoofFx();
Fx.X = this.X + this.W / 2;
Fx.Y = this.Y + this.H / 2;
Dungeon.Area.Add(Fx);
}
}
}
}
public override void Init() {
base.Init();
}
public override void RenderShadow() {
Graphics.Shadow(X, Y + 8, W, H, 14);
}
public override void Render() {
float A = (float) (Math.Cos(this.T + this.Y * 0.1f + this.X * 0.2f) * Am * 6);
Graphics.Render(Region, this.X + 15, this.Y + 2, A, 15, 2, false, false);
}
public Tree() {
_Init();
}
}
}
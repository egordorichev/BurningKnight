using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.level.save;
using BurningKnight.util;

namespace BurningKnight.entity.item {
	public class Lamp : Item {
		public static Lamp Instance;
		private bool Added;
		private bool LightUp;
		private float R;
		public float Val = Dungeon.Type == Dungeon.Type.INTRO ? 90f : 100f;

		protected void _Init() {
			{
				Name = Locale.Get("lamp");
				Sprite = "item-lamp";
				Description = Locale.Get("lamp_desc");
				UseTime = 0.2f;
			}
		}

		public override void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			TextureRegion Sprite = GetSprite();
			var Xx = X + (Flipped ? -W / 2 : W / 2);

			if (R > 0) {
				Graphics.Batch.End();
				Gdx.Gl.GlEnable(GL20.GL_BLEND);
				Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
				Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
				Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
				Graphics.Shape.SetColor(1, 0.5f, 0, 0.4f);
				Graphics.Shape.Circle(X + W / 2 + (Flipped ? -W / 4 : W / 4), Y + Sprite.GetRegionHeight() / 2, (10f + Math.Cos(Dungeon.Time * 3) * Math.Sin(Dungeon.Time * 4)) * R);
				Graphics.Shape.End();
				Gdx.Gl.GlDisable(GL20.GL_BLEND);
				Graphics.Batch.Begin();
			}

			Graphics.Render(Sprite, Xx + Sprite.GetRegionWidth() / 2, Y, 0, Sprite.GetRegionWidth() / 2, 0, Flipped, false);
		}

		public override void SetOwner(Creature Owner) {
			base.SetOwner(Owner);
			Camera.Follow(Player.Instance, false);

			if (!Added && Dungeon.Type != Dungeon.Type.INTRO) {
				Added = true;
				GlobalSave.Put("not_first_time", true);
				PlayerSave.Add(BurningKnight.Instance);
			}
		}

		private static DialogData RandomDialog() {
			switch (Random.NewInt(3)) {
				case 0:
				default: {
					return BurningKnight.ItsYouAgain;
				}

				case 1: {
					return BurningKnight.JustDie;
				}

				case 2: {
					return BurningKnight.NoPoint;
				}
			}
		}

		public override void Init() {
			base.Init();
			Instance = this;
		}

		public override void Use() {
			base.Use();
			LightUp = !LightUp;
			Tween.To(new Tween.Task(LightUp ? 1 : 0, 0.4f) {

		public override float GetValue() {
			return R;
		}

		public override void SetValue(float Value) {
			R = Value;
		}
	});

	Play();
}

public bool IsOn() {
return this.LightUp;
}
public override void Save(FileWriter Writer) {
base.Save(Writer);
Writer.WriteFloat(this.Val);
Writer.WriteBoolean(this.LightUp);
Writer.WriteBoolean(this.Added);
}
public override void Load(FileReader Reader) {
base.Load(Reader);
this.Val = Reader.ReadFloat();
this.LightUp = Reader.ReadBoolean();
this.Added = Reader.ReadBoolean();
if (this.LightUp) {
this.R = 1;
}
}
public override void Update(float Dt) {
base.Update(Dt);
if (this.LightUp) {
if (this.Val > 0) {
this.Val = Math.Max(this.Val - Dt, 0);
Dungeon.Level.AddLightInRadius(this.Owner.X + 8, this.Owner.Y + 8, 2f * (this.Val / 100 + 0.3f), 8f, false);
} else {
this.LightUp = false;
}
}
}
public override StringBuilder BuildInfo() {
StringBuilder Builder = base.BuildInfo();
Builder.Append("\n");
Builder.Append(Math.Round(this.Val));
Builder.Append("% charged");
return Builder;
}
public override void SecondUse() {
base.SecondUse();
if (BurningKnight.Instance != null) {
float D = this.Owner.GetDistanceTo(BurningKnight.Instance.X + BurningKnight.Instance.W / 2, BurningKnight.Instance.Y + BurningKnight.Instance.H / 2) / 16;
if (D < 64f) {
BurningKnight.Instance.Become("fadeOut");
BurningKnight.Instance.AttackTp = true;
}
}
Play();
}
public static void Play() {
Audio.PlaySfx("curse_lamp", 1f, Random.NewFloat(0.9f, 1.9f));
}
public Lamp() {
_Init();
}
}
}
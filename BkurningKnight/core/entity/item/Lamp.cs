using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item {
	public class Lamp : Item {
		protected void _Init() {
			{
				Name = Locale.Get("lamp");
				Sprite = "item-lamp";
				Description = Locale.Get("lamp_desc");
				UseTime = 0.2f;
			}
		}

		public static Lamp Instance;
		public float Val = Dungeon.Type == Dungeon.Type.INTRO ? 90f : 100f;
		private bool LightUp;
		private bool Added;
		private float R;

		public override Void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			TextureRegion Sprite = this.GetSprite();
			float Xx = X + (Flipped ? -W / 2 : W / 2);

			if (this.R > 0) {
				Graphics.Batch.End();
				Gdx.Gl.GlEnable(GL20.GL_BLEND);
				Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
				Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
				Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
				Graphics.Shape.SetColor(1, 0.5f, 0, 0.4f);
				Graphics.Shape.Circle(X + W / 2 + (Flipped ? -W / 4 : W / 4), Y + Sprite.GetRegionHeight() / 2, (float) (10f + Math.Cos(Dungeon.Time * 3) * Math.Sin(Dungeon.Time * 4)) * R);
				Graphics.Shape.End();
				Gdx.Gl.GlDisable(GL20.GL_BLEND);
				Graphics.Batch.Begin();
			} 

			Graphics.Render(Sprite, Xx + Sprite.GetRegionWidth() / 2, Y, 0, Sprite.GetRegionWidth() / 2, 0, Flipped, false);
		}

		public override Void SetOwner(Creature Owner) {
			base.SetOwner(Owner);
			Camera.Follow(Player.Instance, false);

			if (!this.Added && Dungeon.Type != Dungeon.Type.INTRO) {
				this.Added = true;
				GlobalSave.Put("not_first_time", true);
				PlayerSave.Add(creature.mob.boss.BurningKnight.Instance);
			} 
		}

		private static DialogData RandomDialog() {
			switch (Random.NewInt(3)) {
				case 0: 
				default:{
					return creature.mob.boss.BurningKnight.ItsYouAgain;
				}

				case 1: {
					return creature.mob.boss.BurningKnight.JustDie;
				}

				case 2: {
					return creature.mob.boss.BurningKnight.NoPoint;
				}
			}
		}

		public override Void Init() {
			base.Init();
			Instance = this;
		}

		public override Void Use() {
			base.Use();
			this.LightUp = !this.LightUp;
			Tween.To(new Tween.Task(this.LightUp ? 1 : 0, 0.4f) {
				public override float GetValue() {
					return R;
				}

				public override Void SetValue(float Value) {
					R = Value;
				}
			});
			Play();
		}

		public bool IsOn() {
			return this.LightUp;
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteFloat(this.Val);
			Writer.WriteBoolean(this.LightUp);
			Writer.WriteBoolean(this.Added);
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Val = Reader.ReadFloat();
			this.LightUp = Reader.ReadBoolean();
			this.Added = Reader.ReadBoolean();

			if (this.LightUp) {
				this.R = 1;
			} 
		}

		public override Void Update(float Dt) {
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

		public override Void SecondUse() {
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

		public static Void Play() {
			Audio.PlaySfx("curse_lamp", 1f, Random.NewFloat(0.9f, 1.9f));
		}

		public Lamp() {
			_Init();
		}
	}
}

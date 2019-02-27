using BurningKnight.entity.creature.player;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.save;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.level.entities {
	public class Entrance : SaveableEntity {
		public static byte NORMAL = 0;
		public static byte ENTRANCE_TUTORIAL = 1;
		private static TextureRegion Key = Graphics.GetTexture("ui-button_top");
		private float Al;

		private LadderFx Fx;
		private byte Type;

		public void SetType(byte Type) {
			this.Type = Type;
		}

		public byte GetType() {
			return Type;
		}

		public override void Init() {
			base.Init();
			Depth = -3;

			if (Level.GENERATED) AddSelf();

			RegularLevel.Ladder = this;
		}

		public override void Destroy() {
			base.Destroy();
		}

		private void AddSelf() {
			Log.Info("Checking for entrance ladder");

			if (Dungeon.LoadType != LoadType.GO_UP && (Dungeon.LadderId == Type || Player.Ladder == null)) {
				Player.Ladder = this;
				Log.Info("Set entrance ladder!");
			}
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Type = Reader.ReadByte();
			AddSelf();
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteByte(Type);
		}

		public override void Render() {
			if (Dungeon.Depth == 1) {
				DrawKey("W", X - 9, Y + 9);
				DrawKey("A", X + 9, Y + 9);
				DrawKey("S", X - 9, Y - 9);
				DrawKey("D", X + 9, Y - 9);
			}
		}

		private void DrawKey(string Ky, float X, float Y) {
			int Src = Graphics.Batch.GetBlendSrcFunc();
			int Dst = Graphics.Batch.GetBlendDstFunc();
			Graphics.Batch.SetBlendFunction(GL20.GL_DST_COLOR, GL20.GL_ZERO);
			var V = 0.6f;
			Graphics.Batch.SetColor(V, V, V, 1);
			Graphics.Render(Key, X, Y);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			Graphics.Batch.SetBlendFunction(Src, Dst);
			Graphics.SmallSimple.SetColor(1, 1, 1, 0.8f);
			Graphics.Print(Ky, Graphics.SmallSimple, X + 5, Y + 3);
			Graphics.SmallSimple.SetColor(1, 1, 1, 1);
		}

		public override void RenderShadow() {
		}

		public override void OnCollision(Entity Entity) {
			if (Entity is Player && Fx == null && Dungeon.Depth == -2) {
				Fx = new LadderFx(this, "ascend");
				Area.Add(Fx);
			}
		}

		public override void OnCollisionEnd(Entity Entity) {
			if (Entity is Player && Fx != null) {
				Fx.Remove();
				Fx = null;
			}
		}

		private enum LoadType {
			GO_UP,
			GO_DOWN,
			LOADING
		}
	}
}
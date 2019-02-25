using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities {
	public class Entrance : SaveableEntity {
		enum LoadType {
			GO_UP,
			GO_DOWN,
			LOADING
		}

		private LadderFx Fx;
		public static byte NORMAL = 0;
		public static byte ENTRANCE_TUTORIAL = 1;
		private byte Type;
		private float Al;
		private static TextureRegion Key = Graphics.GetTexture("ui-button_top");

		public Void SetType(byte Type) {
			this.Type = Type;
		}

		public byte GetType() {
			return this.Type;
		}

		public override Void Init() {
			base.Init();
			Depth = -3;

			if (Level.GENERATED) {
				this.AddSelf();
			} 

			RegularLevel.Ladder = this;
		}

		public override Void Destroy() {
			base.Destroy();
		}

		private Void AddSelf() {
			Log.Info("Checking for entrance ladder");

			if (Dungeon.LoadType != LoadType.GO_UP && (Dungeon.LadderId == this.Type || Player.Ladder == null)) {
				Player.Ladder = this;
				Log.Info("Set entrance ladder!");
			} 
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Type = Reader.ReadByte();
			this.AddSelf();
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteByte(this.Type);
		}

		public override Void Render() {
			if (Dungeon.Depth == 1) {
				DrawKey("W", X - 9, Y + 9);
				DrawKey("A", X + 9, Y + 9);
				DrawKey("S", X - 9, Y - 9);
				DrawKey("D", X + 9, Y - 9);
			} 
		}

		private Void DrawKey(string Ky, float X, float Y) {
			int Src = Graphics.Batch.GetBlendSrcFunc();
			int Dst = Graphics.Batch.GetBlendDstFunc();
			Graphics.Batch.SetBlendFunction(GL20.GL_DST_COLOR, GL20.GL_ZERO);
			float V = 0.6f;
			Graphics.Batch.SetColor(V, V, V, 1);
			Graphics.Render(Key, X, Y);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			Graphics.Batch.SetBlendFunction(Src, Dst);
			Graphics.SmallSimple.SetColor(1, 1, 1, 0.8f);
			Graphics.Print(Ky, Graphics.SmallSimple, X + 5, Y + 3);
			Graphics.SmallSimple.SetColor(1, 1, 1, 1);
		}

		public override Void RenderShadow() {

		}

		public override Void OnCollision(Entity Entity) {
			if (Entity is Player && this.Fx == null && Dungeon.Depth == -2) {
				this.Fx = new LadderFx(this, "ascend");
				this.Area.Add(this.Fx);
			} 
		}

		public override Void OnCollisionEnd(Entity Entity) {
			if (Entity is Player && this.Fx != null) {
				this.Fx.Remove();
				this.Fx = null;
			} 
		}
	}
}

using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.game.state;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.item {
	public class Item : Entity {
		public static TextureRegion Missing = Graphics.GetTexture("item-missing");
		public float A = 1;
		protected bool Auto = false;
		protected bool AutoPickup;
		protected int Count = 1;
		protected float Delay;
		protected string Description;
		protected string Name;
		protected Creature Owner;
		public int Price = 15;
		protected TextureRegion Region;
		public bool Sale;
		public bool Shop;
		protected string Sprite;
		protected bool Stackable = false;
		protected bool Useable = true;
		public bool UseOnPickup;
		protected string UseSpeedStr;
		protected float UseTime = 0.5f;

		public Item() {
			InitStats();
		}

		protected Item(string Name, string Description, string Sprite) {
			this.Name = Name;
			this.Description = Description;
			this.Sprite = Sprite;
			InitStats();
		}

		public void DisableAutoPickup() {
			AutoPickup = false;
		}

		public int GetPrice() {
			return 5;
		}

		private void InitStats() {
			var UnlocalizedName = Utils.PascalCaseToSnakeCase(GetClass().GetSimpleName());

			if (Sprite == null) Sprite = "item-" + UnlocalizedName;

			if (Name == null) Name = Locale.Get(UnlocalizedName);

			if (Description == null) Description = Locale.Get(UnlocalizedName + "_desc");
		}

		public void UpdateInHands(float Dt) {
		}

		public void Generate() {
		}

		public void OnPickup() {
			if (Dungeon.Depth != -1 && Dungeon.DarkR == Dungeon.MAX_R && Dungeon.Game.GetState() is InGameState) Audio.PlaySfx("pickup_item");

			if (UseOnPickup) {
				Owner = Player.Instance;
				Use();
			}
		}

		public bool IsAuto() {
			return Auto && !Shop;
		}

		public int GetValue() {
			return GetCount();
		}

		public void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
		}

		public override void Update(float Dt) {
			Delay = Math.Max(0, Delay - Dt);
		}

		public Creature GetOwner() {
			return Owner;
		}

		public void SetOwner(Creature Owner) {
			this.Owner = Owner;
		}

		public void Use() {
			Delay = UseTime;
		}

		public void SecondUse() {
			Delay = UseTime;
		}

		public void EndUse() {
		}

		public float GetDelay() {
			return Delay;
		}

		public void Save(FileWriter Writer) {
			Writer.WriteInt32(Count);
			Writer.WriteBoolean(Shop);
			Writer.WriteByte((byte) Price);
			Writer.WriteBoolean(Sale);
		}

		public void Load(FileReader Reader) {
			Count = Reader.ReadInt32();
			Shop = Reader.ReadBoolean();
			Price = Reader.ReadByte();
			Sale = Reader.ReadBoolean();
		}

		public TextureRegion GetSprite() {
			if (Region == null) {
				Region = Graphics.GetTexture(Sprite);

				if (Region == null) {
					Log.Error("Invalid item sprite " + this.GetClass().GetSimpleName());
					Region = Missing;
				}
			}

			return Region;
		}

		public bool IsUseable() {
			return Useable;
		}

		public bool CanBeUsed() {
			return Delay == 0;
		}

		public string GetName() {
			return Count == 0 || Count >= 1 ? Name : Name + " (" + Count + ")";
		}

		public bool IsStackable() {
			return Stackable;
		}

		public int GetCount() {
			return Count;
		}

		public Item SetCount(int Count) {
			this.Count = Count;

			return this;
		}

		public bool HasAutoPickup() {
			return AutoPickup;
		}

		public string GetDescription() {
			return Description;
		}

		public float GetUseTime() {
			return UseTime;
		}

		public StringBuilder BuildInfo() {
			StringBuilder Builder = new StringBuilder();
			Builder.Append(GetName());
			Builder.Append("[gray]");

			if (!Description.IsEmpty()) {
				Builder.Append('\n');
				Builder.Append(GetDescription());
			}

			return Builder;
		}
	}
}
using BurningKnight.core.assets;

namespace BurningKnight.core.util {
	public class Animation {
		public class Frame {
			public TextureRegion Frame;
			public float Delay;
			public float Initial;

			public Frame(TextureRegion Frame, float Delay) {
				this.Frame = Frame;
				this.Delay = Delay;
				this.Initial = Delay;
			}
		}

		private Dictionary<string, List<Frame>> Frames = new Dictionary<>();

		public Animation(string File) {
			this(File, "");
		}

		public List GetFrames<Frame> (string State) {
			return this.Frames.Get(State);
		}

		public Animation(string File, string Add) {
			JsonReader Reader = new JsonReader();
			JsonValue Root = Reader.Parse(Gdx.Files.Internal("sprites_split/" + File + ".json"));
			JsonValue Meta = Root.Get("meta");
			JsonValue FrameTags = Meta.Get("frameTags");
			JsonValue Frames = Root.Get("frames");

			foreach (JsonValue Tag in FrameTags) {
				int From = Tag.GetInt("from");
				int To = Tag.GetInt("to");
				string State = Tag.GetString("name");
				List<Frame> FramesList = new List<>();

				for (int I = From; I <= To; I++) {
					JsonValue Frame = Frames.Get(I);
					string Name = Frame.Name;
					int Delay = Frame.GetInt("duration");
					Name = Name.Replace(".aseprite", "");
					Name = Name.Replace(".ase", "");
					Name = Name.Replace('_', '-');
					Name = Name.Replace(' ', '-');
					Name = Name.Substring(0, Name.Length() - (Character.IsDigit(Name.CharAt(Name.Length() - 2)) ? 3 : 2));
					Name += Add + "-" + State + "-" + string.Format("%02d", I - From);
					FramesList.Add(new Frame(Graphics.GetTexture(Name), Delay * 0.001f));
				}

				this.Frames.Put(State, FramesList);
			}
		}

		public static Animation Make(string File) {
			return new Animation(File);
		}

		public static Animation Make(string File, string Add) {
			return new Animation(File, Add);
		}

		public AnimationData Get(string State) {
			List<Frame> Data = this.Frames.Get(State);

			if (Data == null) {
				return null;
			} 

			return new AnimationData(Data);
		}
	}
}

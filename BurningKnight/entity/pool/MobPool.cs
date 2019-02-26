using System;
using System.Collections.Generic;
using BurningKnight.util;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.pool {
	public class MobPool {
		public static MobPool Instance = new MobPool();
		protected List<float> Chances = new List<float>();
		protected List<MobHub> Classes = new List<MobHub>();
		protected List<float> Dchances = new List<float>();
		protected List<MobHub> Dclasses = new List<MobHub>();

		public void InitForRoom() {
			for (var I = 0; I < Dchances.Count; I++) {
				var Cl = Dclasses[I];
				
				Classes.Add(Cl);
				Chances.Add(Dchances[I]);
				Cl.MaxMatches = Cl.MaxMatchesInitial;
			}

			Dchances.Clear();
			Dclasses.Clear();
		}

		public MobHub Generate() {
			var I = Random.Chances(Chances);

			if (I == -1) {
				Log.Error("-1 as pool result!");

				return null;
			}

			var Hub = Classes[I];

			if (Hub != null) {
				Hub.MaxMatches -= 1;

				if (Hub.MaxMatches == 0) {
					if (!Hub.Once) {
						Dchances.Add(Chances[Classes.IndexOf(Hub)]);
						Dclasses.Add(Hub);
					}

					Chances.Remove(Classes.IndexOf(Hub));
					Classes.Remove(Hub);
				}
			}

			return Hub;
		}

		public void Add(MobHub Type, float Chance) {
			Classes.Add(Type);
			Chances.Add(Chance);
		}

		public void Clear() {
			Classes.Clear();
			Chances.Clear();
		}

		private void Add(float Chance, int Max, params Type[] Classes) {
			Add(new MobHub(Chance, Max, Classes), Chance);
		}

		public void InitForFloor() {
			Clear();
		}
	}
}
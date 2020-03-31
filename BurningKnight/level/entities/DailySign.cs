using System;
using BurningKnight.entity.component;
using BurningKnight.state;
using Lens.assets;
using Lens.util;

namespace BurningKnight.level.entities {
	public class DailySign : Sign {
		public DailySign() {
			Region = "sign_daily";
		}

		private string GetTime(TimeSpan span) {
			Log.Debug(span);

			if ((int) span.TotalHours > 1) {
				return $"{(int) span.TotalHours} {Locale.Get("hours")}";
			}

			if (span.Minutes > 1) {
				return $"{span.Minutes} {Locale.Get("minutes")}";
			}
			
			return $"{span.Seconds} {Locale.Get("seconds")}";
		}

		public override void AddComponents() {
			base.AddComponents();

			GetComponent<CloseDialogComponent>().DecideVariant = (e) => {
				var d = DateTime.UtcNow;
				var dd = d.AddDays(1);
				
				var next = new DateTime(dd.Year, dd.Month, dd.Day, 0, 0, 0, DateTimeKind.Utc);

				Log.Debug(d);
				Log.Debug(next);

				return $"[sp 3]{Locale.Get("run_daily")} #{Run.CalculateDailyId()}\n{Locale.Get("next_daily_in")}\n{GetTime(next.Subtract(d))}";
			};
		}
	}
}
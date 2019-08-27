using System;
using System.Diagnostics;

namespace Lens.util {
	public static class Web {
		public static void Open(string url) {
			try {
				Log.Info($"Navigating to {url}");
				var webPage = new ProcessStartInfo(url);
				Process.Start(webPage);
			} catch (Exception e) {
				Log.Error(e);
			}
		}
	}
}
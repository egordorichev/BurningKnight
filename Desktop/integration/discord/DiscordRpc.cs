using System;
using System.Runtime.InteropServices;

namespace Desktop.integration.discord {
	public class DiscordRpc {
		public struct EventHandlers {
			public IntPtr ready;
			public IntPtr disconnected;
			public IntPtr errored;
			public IntPtr joinGame;
			public IntPtr spectateGame;
			public IntPtr joinRequest;
		}

		//--------------------------------------------------------------------------------

		public struct RichPresence {
			public string state;
			public string details;
			public Int64 startTimestamp;
			public Int64 endTimestamp;
			public string largeImageKey;
			public string largeImageText;
			public string smallImageKey;
			public string smallImageText;
			public string partyId;
			public int partySize;
			public int partyMax;
			public string matchSecret;
			public string joinSecret;
			public string spectateSecret;
			public sbyte instance;
		}

		//--------------------------------------------------------------------------------
		
		public struct DiscordUser {
			public string userId;
			public string username;
			public string discriminator;
			public string avatar;
		}
		
		public struct JoinRequest {
			public string userId;
			public string username;
			public string avatar;
		}

		//--------------------------------------------------------------------------------

		public enum Reply : int {
			No = 0,
			Yes = 1,
			Ignore = 2
		}

		//--------------------------------------------------------------------------------

		[DllImport("discord-rpc.dll")]
		private static extern void Discord_Initialize([MarshalAs(UnmanagedType.LPStr)] string applicationID,
			ref EventHandlers handlers,
			int autoRegister,
			[MarshalAs(UnmanagedType.LPStr)] string optionalSteamId);

		public static void Initialize(string appID, EventHandlers handlers) {
			Discord_Initialize(appID, ref handlers, 1, String.Empty);
		}

		//--------------------------------------------------------------------------------

		[DllImport("discord-rpc.dll")]
		private static extern void Discord_UpdatePresence(IntPtr presence);

		public static void UpdatePresence(RichPresence presence) {
			IntPtr ptrPresence = Marshal.AllocHGlobal(Marshal.SizeOf(presence));
			Marshal.StructureToPtr(presence, ptrPresence, false);
			Discord_UpdatePresence(ptrPresence);
		}

		//--------------------------------------------------------------------------------

		[DllImport("discord-rpc.dll")]
		private static extern void Discord_Shutdown();

		public static void Shutdown() {
			Discord_Shutdown();
		}

		//--------------------------------------------------------------------------------

		[DllImport("discord-rpc.dll")]
		private static extern void Discord_UpdateConnection();

		public static void UpdateConnection() {
			Discord_UpdateConnection();
		}

		//--------------------------------------------------------------------------------

		[DllImport("discord-rpc.dll")]
		private static extern void Discord_RunCallbacks();

		public static void RunCallbacks() {
			Discord_RunCallbacks();
		}

		//--------------------------------------------------------------------------------

		[DllImport("discord-rpc.dll")]
		private static extern void Discord_Respond(string userId, int reply);

		public static void Respond(string userID, Reply reply) {
			Discord_Respond(userID, (int) reply);
		}
	}
}
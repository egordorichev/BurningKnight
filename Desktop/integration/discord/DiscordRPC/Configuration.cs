using Newtonsoft.Json;

namespace Desktop.integration.discord.DiscordRPC
{
	/// <summary>
	/// Configuration of the current RPC connection
	/// </summary>
	public class Configuration
	{
		/// <summary>
		/// The Discord API endpoint that should be used.
		/// </summary>
		[JsonProperty("api_endpoint")]
		public string ApiEndpoint { get; set; }

		/// <summary>
		/// The CDN endpoint
		/// </summary>
		[JsonProperty("cdn_host")]
		public string CdnHost { get; set; }

		/// <summary>
		/// The type of enviroment the connection on. Usually Production. 
		/// </summary>
		[JsonProperty("enviroment")]
		public string Enviroment { get; set; }
	}
}

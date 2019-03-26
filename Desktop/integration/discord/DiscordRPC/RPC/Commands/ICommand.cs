using Desktop.integration.discord.DiscordRPC.RPC.Payload;

namespace Desktop.integration.discord.DiscordRPC.RPC.Commands
{
    internal interface ICommand
	{
		IPayload PreparePayload(long nonce);
	}
}

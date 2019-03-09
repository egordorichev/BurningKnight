using System;

namespace BurningKnight.integration.discord.NamedPipeClient.IO.Exceptions
{
    public class NamedPipeConnectionException : Exception
    {
        internal NamedPipeConnectionException(string message) : base(message) { }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Lens.assets.Contracts.Ogg;

namespace Lens.assets.Ogg
{
    class ForwardOnlyPageReader : PageReaderBase
    {
        internal static Func<IPageReader, int, IForwardOnlyPacketProvider> CreatePacketProvider { get; set; } = (pr, ss) => new ForwardOnlyPacketProvider(pr, ss);

        private readonly Dictionary<int, IForwardOnlyPacketProvider> _packetProviders = new Dictionary<int, IForwardOnlyPacketProvider>();
        private readonly Func<Contracts.IPacketProvider, bool> _newStreamCallback;

        public ForwardOnlyPageReader(Stream stream, bool closeOnDispose, Func<Contracts.IPacketProvider, bool> newStreamCallback)
            : base(stream, closeOnDispose)
        {
            _newStreamCallback = newStreamCallback;
        }

        protected override bool AddPage(int streamSerial, byte[] pageBuf, bool isResync)
        {
            if (_packetProviders.TryGetValue(streamSerial, out var pp))
            {
                // try to add the page...
                if (pp.AddPage(pageBuf, isResync))
                {
                    // ..., then make sure we're not flagged as the end of the stream...
                    if (((PageFlags)pageBuf[5] & PageFlags.EndOfStream) == 0)
                    {
                        // ... in which case we say we're good.
                        return true;
                    }
                }
                // otherwise, remove the stream from our list and fall through to:
                _packetProviders.Remove(streamSerial);
            }

            // try to add the stream to the list.
            pp = CreatePacketProvider(this, streamSerial);
            pp.AddPage(pageBuf, isResync);
            _packetProviders.Add(streamSerial, pp);
            if (_newStreamCallback(pp))
            {
                return true;
            }
            _packetProviders.Remove(streamSerial);
            return false;
        }

        protected override void SetEndOfStreams()
        {
            foreach (var kvp in _packetProviders)
            {
                kvp.Value.SetEndOfStream();
            }
            _packetProviders.Clear();
        }

        public override bool ReadPageAt(long offset) => throw new NotSupportedException();
    }
}

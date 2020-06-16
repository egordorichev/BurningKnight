namespace Lens.assets.Contracts.Ogg
{
    interface IForwardOnlyPacketProvider : IPacketProvider
    {
        bool AddPage(byte[] buf, bool isResync);
        void SetEndOfStream();
    }
}

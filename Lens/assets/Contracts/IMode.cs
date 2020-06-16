namespace Lens.assets.Contracts
{
    interface IMode
    {
        int BlockSize { get; }
        float[][] Windows { get; }

        void Init(IPacket packet, int channels, int block0Size, int block1Size, IMapping[] mappings);

        bool Decode(IPacket packet, float[][] buffer, out int packetStartindex, out int packetValidLength, out int packetTotalLength);

        int GetPacketSampleCount(IPacket packet, bool isFirst);
    }
}

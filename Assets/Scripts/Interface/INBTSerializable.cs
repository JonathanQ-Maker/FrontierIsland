
using NBT.Tags;

namespace FrontierIsland
{
    public interface INBTSerializable
    {
        void ReadFromNBT(CompoundTag nbt);
        void WriteToNBT(CompoundTag nbt);
    }
}
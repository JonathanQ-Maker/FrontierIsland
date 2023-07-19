
using NBT.Tags;

namespace FrontierIsland
{
    public interface INBTSerializable<T> where T : Tag
    {
        T SerializeNBT();
        void DeserializeNBT(T tag);
    }
}
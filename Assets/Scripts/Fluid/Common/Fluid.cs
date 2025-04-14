using NBT.Tags;

namespace FrontierIsland
{
    public abstract class Fluid : INBTSerializable, ICopyable<Fluid>
    {

        public abstract string Name { get; }

        public abstract FluidType FluidType { get; }

        public string description;

        public int amount;

        public Fluid(int amount, string description)
        {
            this.amount = amount;
            this.description = description;
        }

        public virtual void ReadFromNBT(CompoundTag nbt)
        {
            
        }

        public virtual void WriteToNBT(CompoundTag nbt)
        {
            
        }

        public virtual void CopyTo(Fluid other)
        {
            other.description = description;
            other.amount = amount;
        }

        public abstract Fluid DeepClone();

        public virtual bool Similar(Fluid other) 
        {
            //if (HasNBT)
            //{
            //    if (item.HasNBT)
            //    {
            //        return ItemType == item.ItemType && item.NBT.Equals(NBT);
            //    }
            //    return false;
            //}
            return other != null && FluidType == other.FluidType;
        }
    }
}

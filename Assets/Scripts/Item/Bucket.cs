using UnityEngine;

namespace FrontierIsland
{
    public abstract class Bucket : ItemStack, IFluidContainerItem, ICopyable<Bucket>
    {
        /// <summary>
        /// <br>
        /// Amount of fluid a bucket can hold
        /// </br>
        /// <br>
        /// Unit in mb (milibucket)
        /// </br>
        /// </summary>
        public const int BUCKET_VOLUME = 1000;
        protected Fluid fluid;

        protected Bucket(int count, string name, string description) : base(count, name, description)
        {

        }

        public virtual int Capacity { get { return BUCKET_VOLUME; } }

        public void CopyTo(Bucket other)
        {
            CopyTo((ItemStack)other);
            other.fluid = fluid.DeepClone();
        }

        public virtual Fluid Drain(int maxDrain)
        {
            if (fluid != null && maxDrain > 0)
            {
                Fluid result = fluid.DeepClone();
                result.amount = Mathf.Min(maxDrain, fluid.amount);
                fluid.amount -= result.amount;
                if (fluid.amount <= 0)
                {
                    fluid = null;
                }
                return result;
            }
            return null;
        }

        public virtual int Fill(Fluid other, int maxFill)
        {
            if (other.amount <= 0)
            {
                return 0;
            }


            if (fluid == null)
            {
                // fill null bucket
                fluid = other.DeepClone();
                fluid.amount = Mathf.Min(other.amount, maxFill, Capacity);
                return fluid.amount;
            }
            else if (fluid.Similar(other))
            {
                // fill existing bucket
                int amount = Mathf.Min(Capacity - fluid.amount, other.amount, maxFill);
                fluid.amount += amount; 
                return amount;
            }
            return 0;
        }

        public override string GetToolTip()
        {
            return $"<size=14><color=#C5D4E9>{name}<size=12>\n" +
                   (FuelValue > 0 ? $"Fuel Value: {FuelValue}\n" : "") +
                   $"Volume: {(fluid == null ? 0 : fluid.amount)}mb\n" +
                   $"<color=#898989><i>{description}</i></color>\n";
        }
    }
}

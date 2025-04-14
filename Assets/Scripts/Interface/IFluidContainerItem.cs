namespace FrontierIsland
{
    public interface IFluidContainerItem
    {
        int Capacity { get; }

        /// <summary>
        /// Attempt to fill this <see cref="IFluidContainerItem"/> with <paramref name="fluid"/>
        /// </summary>
        /// <param name="fluid"></param>
        /// <returns>amount of fluid filled</returns>
        int Fill(Fluid fluid, int maxFill);

        /// <summary>
        /// Drains <see cref="Fluid"/> in this <see cref="IFluidContainerItem"/>
        /// </summary>
        /// <param name="maxDrain">
        /// Maximum amount of fluid to be removed from the container
        /// </param>
        /// <returns>drained content</returns>
        Fluid Drain(int maxDrain);
    }
}

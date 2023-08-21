using System.Collections;

namespace FrontierIsland
{
    public interface IUseable
    {
        Settler.AnimState UseState { get; }
    }
}

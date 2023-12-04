using DAT;
using Unity.Collections;
using Unity.Entities;

public readonly partial struct Base_InputAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRW<Base_Tag> _base_Tag;

    public void DOSOMETHING()
    {
        //DOSOMETHING
    }
}

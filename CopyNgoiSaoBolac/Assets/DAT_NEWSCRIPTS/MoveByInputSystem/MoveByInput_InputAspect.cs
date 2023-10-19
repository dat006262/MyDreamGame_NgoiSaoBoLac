using DAT;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct MoveByInput_InputAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRW<MoveByInput_Speed> _moveSpeed;
    public readonly RefRO<PlayerInput_OutDataComponent> input;
    //=============================================
    public void DOSOMETHING()
    {

    }
}

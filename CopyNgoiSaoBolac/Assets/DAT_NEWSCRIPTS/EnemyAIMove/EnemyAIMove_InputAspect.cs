
using DAT;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct EnemyAIMove_InputAspect : IAspect
{
    public readonly Entity Entity;

    public readonly RefRW<EnemyAIMove_Speed> _moveSpeed;
    public readonly RefRO<EnemyAIMove_TargetTo> playerEnt;
    public readonly RefRW<EnemyAIMove_Distances> distances;
    //=============================================
    public void DOSOMETHING()
    {

    }
}

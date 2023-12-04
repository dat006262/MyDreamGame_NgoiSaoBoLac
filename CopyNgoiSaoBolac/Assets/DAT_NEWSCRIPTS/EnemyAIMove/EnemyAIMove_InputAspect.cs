
using DAT;
using Unity.Collections;
using Unity.Entities;

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

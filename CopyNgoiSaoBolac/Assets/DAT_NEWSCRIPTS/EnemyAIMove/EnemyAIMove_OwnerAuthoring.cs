using DAT;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyAIMove_OwnerAuthoring : MonoBehaviour
{
    public float speed = 100;
    public class EnemyAIMove_OwnerBaker : Baker<EnemyAIMove_OwnerAuthoring>
    {
        public override void Bake(EnemyAIMove_OwnerAuthoring authoring)
        {
            AddComponent<EnemyAIMoveTag>();
            AddComponent<EnemyAIMove_Speed>(new EnemyAIMove_Speed { speed = authoring.speed });
            AddComponent<EnemyAIMove_Distances>();
            AddComponent<EnemyAIMove_CheckIsRight>();
            AddComponent<EnemyAIMove_TargetTo>();
        }
    }
}

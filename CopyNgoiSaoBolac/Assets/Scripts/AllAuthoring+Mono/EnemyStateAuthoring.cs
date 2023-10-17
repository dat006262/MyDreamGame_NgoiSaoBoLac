using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyStateAuthoring : MonoBehaviour
{

    //public EnemyStateComponent.State state = EnemyStateComponent.State.FindingTarget;


    public class EnemyStateBaker : Baker<EnemyStateAuthoring>
    {
        public override void Bake(EnemyStateAuthoring authoring)
        {
            AddComponent<EnemyStateComponent>(new EnemyStateComponent
            {
                state = EnemyStateComponent.State.Running
            });
        }
    }
}

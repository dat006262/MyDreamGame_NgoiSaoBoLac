using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Q_MundoSpawnSys_OwnerAuthoring : MonoBehaviour
{
    public bool active = true;
    public GameObject prefab;
    public class SpawnLineSkillSys_OwnerBaker : Baker<Q_MundoSpawnSys_OwnerAuthoring>
    {

        public override void Bake(Q_MundoSpawnSys_OwnerAuthoring authoring)
        {
            AddComponent<SpawnLineSkillSys_OwnerComponent>(new SpawnLineSkillSys_OwnerComponent
            {
                active = authoring.active,
                prefab = GetEntity(authoring.prefab),
                timeToLive = 0,
            }
            );
        }
    }
}

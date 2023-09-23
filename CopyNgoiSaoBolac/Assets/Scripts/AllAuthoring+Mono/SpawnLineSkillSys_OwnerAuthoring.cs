using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnLineSkillSys_OwnerAuthoring : MonoBehaviour
{
    public bool active = true;
    public GameObject prefab;
    public class SpawnLineSkillSys_OwnerBaker : Baker<SpawnLineSkillSys_OwnerAuthoring>
    {

        public override void Bake(SpawnLineSkillSys_OwnerAuthoring authoring)
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

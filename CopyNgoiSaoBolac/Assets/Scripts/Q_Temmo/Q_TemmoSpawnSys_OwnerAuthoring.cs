using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Q_TemmoSpawnSys_OwnerAuthoring : MonoBehaviour
{
    public bool active = true;
    public GameObject prefab;
    public float RangleSkill = 15;
    public class SkillAutoTargetSys_OwnerBaker : Baker<Q_TemmoSpawnSys_OwnerAuthoring>
    {

        public override void Bake(Q_TemmoSpawnSys_OwnerAuthoring authoring)
        {
            AddComponent<SkillAutoTargetSys_OwnerComponent>(new SkillAutoTargetSys_OwnerComponent
            {
                active = authoring.active,
                prefab = GetEntity(authoring.prefab),
                RangeSkill = authoring.RangleSkill
            }
            );
        }
    }
}

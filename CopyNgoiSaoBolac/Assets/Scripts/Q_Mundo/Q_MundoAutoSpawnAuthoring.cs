using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Q_MundoAutoSpawnAuthoring : MonoBehaviour
{
    public bool active = true;
    public GameObject prefab;
    public float cooldown = 2f;
    public class Q_MundoAutoSpawn_OwnerBaker : Baker<Q_MundoAutoSpawnAuthoring>
    {

        public override void Bake(Q_MundoAutoSpawnAuthoring authoring)
        {
            AddComponent<Q_MundoAutoSpawnComponent>(new Q_MundoAutoSpawnComponent
            {
                active = authoring.active,
                prefab = GetEntity(authoring.prefab),
                cooldownTime = authoring.cooldown
            }
            );
        }
    }
}
public struct Q_MundoAutoSpawnComponent : IComponentData
{
    public bool active;
    public Entity prefab;
    public float cooldownTime;
    public float cooldowmRemain;
}

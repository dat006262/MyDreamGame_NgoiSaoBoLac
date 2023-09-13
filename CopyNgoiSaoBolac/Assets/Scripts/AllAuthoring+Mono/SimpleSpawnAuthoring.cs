using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SimpleSpawnAuthoring : MonoBehaviour
{
    public uint spawnNumber = 1;
    public class AsteroidPrefabBaker : Baker<SimpleSpawnAuthoring>
    {

        public override void Bake(SimpleSpawnAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent<SimpleSpawner_OwnerComponent>(entity, new SimpleSpawner_OwnerComponent
            {
                spawnNumber = authoring.spawnNumber
            });
        }
    }
}

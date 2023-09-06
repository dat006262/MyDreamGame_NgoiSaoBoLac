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
            AddComponent<SimpleSpawnerComponent>(entity, new SimpleSpawnerComponent
            {
                spawnNumber = authoring.spawnNumber
            });
        }
    }
}

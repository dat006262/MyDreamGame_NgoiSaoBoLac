using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PrefabAndParentAuthoring : MonoBehaviour
{
    [Header("One or more.You can leave the parent empty.")]
    public GameObject[] prefabs;
    public GameObject[] parents;

    public class AsteroidPrefabBaker : Baker<PrefabAndParentAuthoring>
    {
        public override void Bake(PrefabAndParentAuthoring authoring)
        {
            //var entity = GetEntity(TransformUsageFlags.None);
            var buffer = AddBuffer<SimpleSpawner_PrefabAndParentBufferComponent>(/*entity*/);
            for (int i = 0; i < authoring.prefabs.Length; i++)
            {
                DependsOn(authoring.prefabs[i]);

                buffer.Add(new SimpleSpawner_PrefabAndParentBufferComponent
                {
                    prefab = GetEntity(authoring.prefabs[i]/*, TransformUsageFlags.Dynamic*/),
                    parent = GetEntity(authoring.parents[i]/*, TransformUsageFlags.None*/)
                });
            }
        }
    }
}

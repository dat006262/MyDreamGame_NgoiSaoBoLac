using DAT;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RandomSpawnEnemy_OwnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject Parent;

    public int spawnPerWave = 5;
    public float waveTimeColdDown = 5;

    public float3[] PosSpawm;

    public class RandomSpawnEnemy_OwnerBaker : Baker<RandomSpawnEnemy_OwnerAuthoring>
    {
        public override void Bake(RandomSpawnEnemy_OwnerAuthoring authoring)
        {
            AddComponent<RandomSpawnEnemyTag>();
            AddComponent<RandomSpawnEnemy_Prefab>(new RandomSpawnEnemy_Prefab
            {
                prefab = GetEntity(authoring.Prefab),
                parent = GetEntity(authoring.Parent)
            });
            AddComponent<RandomSpawnEnemy_RandomValue>();
            AddComponent<RandomSpawnEnemy_SpawmWave>(new RandomSpawnEnemy_SpawmWave
            {
                spawnPerWave = authoring.spawnPerWave,
                waveTimeColdDown = authoring.waveTimeColdDown
            });
            var buffer = AddBuffer<RandomSpawnEnemy_SpawmPos>();
            for (int i = 0; i < authoring.PosSpawm.Length; i++)
            {
                buffer.Add(new RandomSpawnEnemy_SpawmPos
                {
                    spawnPos = authoring.PosSpawm[i]/*, TransformUsageFlags.None*/
                });
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RandomSpawnAuthoring : MonoBehaviour
{
    public int spawnPerWave;
    public float waveTimeColdDown;

    public float3[] PosSpawm;

    public class RandomSpawnBaker : Baker<RandomSpawnAuthoring>
    {
        public override void Bake(RandomSpawnAuthoring authoring)
        {
            AddComponent<RandomSpawnComponent>(new RandomSpawnComponent
            {
                spawnPerWave = authoring.spawnPerWave,
                waveTimeColdDown = authoring.waveTimeColdDown,
                waveTimeRemain = authoring.waveTimeColdDown
            });
            var entity = GetEntity(authoring);
            var buffer = AddBuffer<RandomSpawnPosBufferElement>(/*entity*/);
            for (int i = 0; i < authoring.PosSpawm.Length; i++)
            {
                buffer.Add(new RandomSpawnPosBufferElement
                {
                    sumPos = authoring.PosSpawm[i]/*, TransformUsageFlags.None*/
                });
            }
        }

    }
}
public struct RandomSpawnComponent : IComponentData
{
    public int spawnPerWave;
    public float waveTimeColdDown;
    public float waveTimeRemain;
}
public struct RandomSpawnPosBufferElement : IBufferElementData
{

    public float3 sumPos;
}
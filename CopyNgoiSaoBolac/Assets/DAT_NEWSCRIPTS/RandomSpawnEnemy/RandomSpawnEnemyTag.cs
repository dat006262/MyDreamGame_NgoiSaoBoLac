using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace DAT
{
    //============================TAG================================
    public struct RandomSpawnEnemyTag : IComponentData { }
    //==========================Input================================
    public struct RandomSpawnEnemy_Prefab : IComponentData
    {
        public Entity prefab;
        public Entity parent;
    }
    public struct RandomSpawnEnemy_SpawmPos : IBufferElementData
    {
        public float3 spawnPos;
    }
    public struct RandomSpawnEnemy_SpawmWave : IComponentData
    {
        public int spawnPerWave;
        public float waveTimeColdDown;
    }
    //==========================CreateOnWork======================= 
    public struct RandomSpawnEnemy_RandomValue : IComponentData
    {
        public Unity.Mathematics.Random Value;
    }
    //==========================Output==========================

}
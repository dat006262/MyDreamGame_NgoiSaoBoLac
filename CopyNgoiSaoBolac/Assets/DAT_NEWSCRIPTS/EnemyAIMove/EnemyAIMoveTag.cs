using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.Build;
using UnityEngine;

namespace DAT
{
    //Input la list Pos All Playe
    public struct EnemyAIMoveTag : IComponentData
    {

    }
    public struct EnemyAIMove_Speed : IComponentData
    {
        public float speed;
        public float3 distances;
    }
    public struct EnemyAIMove_Distances : IComponentData
    {

        public float3 distances;
    }
    public struct EnemyAIMove_CheckIsRight : IComponentData
    {
        public bool isRight;
    }
    public struct EnemyAIMove_TargetTo : IComponentData
    {
        public Entity playerEntity;
        public float3 PlayerPos;

    }
}

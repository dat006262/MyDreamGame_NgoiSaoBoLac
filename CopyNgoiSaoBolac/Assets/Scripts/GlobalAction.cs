using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class GlobalAction
{
    //-----------------------------------------
    public static Action<float, float3> OnUpdateHealth;
    public static Action<float> OnGrantEXP;
    public static Action OnLevelUp;
    public static Action<EntityCommandBuffer, Entity, Entity> OnEnemyReceiveHit;
}

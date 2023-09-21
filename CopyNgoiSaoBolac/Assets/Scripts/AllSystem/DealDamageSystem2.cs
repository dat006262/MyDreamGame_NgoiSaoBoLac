using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class DealDamageSystem2 : SystemBase
{
    private EntityQuery m_dealDamage;
    public Action<float, float3> OnDealDamage;
    public Action<float, float3> OnGrantExperience;
    protected override void OnCreate()
    {
        RequireForUpdate<DealDamageSystem2Enable>();
    }
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);




        foreach (var (health, damageBuffer, transform, entity) in
              SystemAPI.Query<RefRW<ExecuteTriggerSys_HealthComponent>, DynamicBuffer<DealDamageSys2_OwnerComponent>,
                  LocalTransform>().WithEntityAccess())
        {
            for (var i = damageBuffer.Length - 1; i >= 0; i--)
            {
                if (damageBuffer.ElementAt(i).CandealDamage)
                {
                    health.ValueRW.Value -= damageBuffer.ElementAt(i).Value;
                    OnDealDamage?.Invoke(damageBuffer.ElementAt(i).Value, transform.Position);
                    damageBuffer.ElementAt(i).loopCount--;
                    damageBuffer.ElementAt(i).effectCount = damageBuffer.ElementAt(i).effectFrequenc;

                }
                if (damageBuffer.ElementAt(i).CanRemove)
                {
                    damageBuffer.RemoveAt(i);
                }
            }

        }



    }
}

using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class DealDamageSystem : SystemBase
{
    public Action<float, float3> OnDealDamage;
    public Action<float, float3> OnGrantExperience;
    protected override void OnCreate()
    {
        RequireForUpdate<DealDamageSystemEnable>();
    }
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // For each character with a damage component...
        foreach (var (hitPoints, damageToCharacter, experiencePoints, transform, entity) in
                 SystemAPI.Query<RefRW<DamageSys_HealthComponent>, RefRW<DealDamageSys_OwnerComponent>, DealDamageSys_EXPComponent,
                     LocalTransform>().WithEntityAccess())
        {
            if (EntityManager.HasComponent<DealDamageSys_EffectCountDownComponent>(entity))
            {

                DealDamageSys_EffectCountDownComponent x = SystemAPI.GetComponent<DealDamageSys_EffectCountDownComponent>(entity);

                if (!x.canDamage)
                {

                    //    x.effectCountDown -= SystemAPI.Time.DeltaTime;
                    ecb.SetComponent<DealDamageSys_EffectCountDownComponent>(entity, new DealDamageSys_EffectCountDownComponent { effectCountDown = x.effectCountDown - SystemAPI.Time.DeltaTime });

                }
                else
                {
                    hitPoints.ValueRW.Value -= damageToCharacter.ValueRW.Value;
                    OnDealDamage?.Invoke(damageToCharacter.ValueRW.Value, transform.Position);
                    ecb.RemoveComponent<DealDamageSys_EffectCountDownComponent>(entity);
                    ecb.RemoveComponent<DealDamageSys_OwnerComponent>(entity);
                }
            }
            else
            {

                ecb.AddComponent<DealDamageSys_EffectCountDownComponent>(entity, new DealDamageSys_EffectCountDownComponent { effectCountDown = damageToCharacter.ValueRO.effectFrequenc });

            }



            // If the damaged character is out of health... Add experience to the player
            if (hitPoints.ValueRO.Value <= 0)
            {
                ecb.AddComponent<DeadDestroyTag>(entity, new DeadDestroyTag { DeadAfter = 0 });
            }
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

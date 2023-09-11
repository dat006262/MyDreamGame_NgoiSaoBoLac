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
                 SystemAPI.Query<RefRW<CharacterHitPoints>, RefRW<DamageToCharacter>, CharacterExperiencePoints,
                     LocalTransform>().WithEntityAccess())
        {
            if (EntityManager.HasComponent<SkillEffectComponent>(entity))
            {

                SkillEffectComponent x = SystemAPI.GetComponent<SkillEffectComponent>(entity);

                if (!x.canDamage)
                {

                    //    x.effectCountDown -= SystemAPI.Time.DeltaTime;
                    ecb.SetComponent<SkillEffectComponent>(entity, new SkillEffectComponent { effectCountDown = x.effectCountDown - SystemAPI.Time.DeltaTime });

                }
                else
                {
                    hitPoints.ValueRW.Value -= damageToCharacter.ValueRW.Value;
                    OnDealDamage?.Invoke(damageToCharacter.ValueRW.Value, transform.Position);
                    ecb.RemoveComponent<SkillEffectComponent>(entity);
                    ecb.RemoveComponent<DamageToCharacter>(entity);
                }
            }
            else
            {

                ecb.AddComponent<SkillEffectComponent>(entity, new SkillEffectComponent { effectCountDown = damageToCharacter.ValueRO.effectFrequenc });

            }



            // If the damaged character is out of health... Add experience to the player
            if (hitPoints.ValueRO.Value <= 0)
            {
                // ecb.DestroyEntity(entity);
                ecb.AddComponent<DeadDestroyTag>(entity);
                //var originCharacterExperience =
                //    SystemAPI.GetComponent<CharacterExperiencePoints>(damageToCharacter.OriginCharacter);
                //originCharacterExperience.Value += experiencePoints.Value;
                //SystemAPI.SetComponent(damageToCharacter.OriginCharacter, originCharacterExperience);

                //var originCharacterPosition =
                //    SystemAPI.GetComponent<LocalTransform>(damageToCharacter.OriginCharacter).Position;
                //OnGrantExperience?.Invoke(experiencePoints.Value, originCharacterPosition);
            }
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

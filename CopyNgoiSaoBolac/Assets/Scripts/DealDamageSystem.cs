using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class DealDamageSystem : SystemBase
{
    public Action<int, float3> OnDealDamage;
    public Action<int, float3> OnGrantExperience;
    protected override void OnCreate()
    {
        RequireForUpdate<DealDamageSystemEnable>();
    }
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // For each character with a damage component...
        foreach (var (hitPoints, damageToCharacter, experiencePoints, transform, entity) in
                 SystemAPI.Query<RefRW<CharacterHitPoints>, DamageToCharacter, CharacterExperiencePoints,
                     LocalTransform>().WithEntityAccess())
        {
            // Subtract health from the character
            hitPoints.ValueRW.Value -= damageToCharacter.Value;

            // Invoke the OnDealDamage event, passing in the required data
            OnDealDamage?.Invoke(damageToCharacter.Value, transform.Position);

            ecb.RemoveComponent<DamageToCharacter>(entity);

            // If the damaged character is out of health... Add experience to the player
            if (hitPoints.ValueRO.Value <= 0)
            {
                ecb.DestroyEntity(entity);
                var originCharacterExperience =
                    SystemAPI.GetComponent<CharacterExperiencePoints>(damageToCharacter.OriginCharacter);
                originCharacterExperience.Value += experiencePoints.Value;
                SystemAPI.SetComponent(damageToCharacter.OriginCharacter, originCharacterExperience);

                var originCharacterPosition =
                    SystemAPI.GetComponent<LocalTransform>(damageToCharacter.OriginCharacter).Position;
                OnGrantExperience?.Invoke(experiencePoints.Value, originCharacterPosition);
            }
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

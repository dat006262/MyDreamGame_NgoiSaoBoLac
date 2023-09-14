
using System;
using Unity.Entities;
using UnityEngine;
public class ConfigExecute : MonoBehaviour
{

    [Header("AllSystemEnable")]
    public bool SpawnSimpleSystem;
    public bool PlayerInputSystemEnable;
    public bool PlayerMovementSystemEnable;
    public bool EnemyAISystemEnable;
    public bool HackInputSystemEnable;
    public bool HackSystemEnable;
    public bool ApplyModifySystemEnable;
    public bool SkillCoolDownSystemEnable;
    public bool DealDamageSystemEnable;
    public bool DamageSystemEnable;
    public bool DeadDestroySystemEnable;
    public bool PlayerProjectileSystemEnable;
    public bool TargetToEnemySystemEnable;
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    class Baker : Baker<ConfigExecute>
    {
        public override void Bake(ConfigExecute authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            if (authoring.SpawnSimpleSystem) AddComponent<SpawnSimpleSystemEnable>(entity);
            if (authoring.PlayerInputSystemEnable) AddComponent<PlayerInputSystemEnable>(entity);
            if (authoring.PlayerMovementSystemEnable) AddComponent<PlayerMovementSystemEnable>(entity);
            if (authoring.EnemyAISystemEnable) AddComponent<EnemyAISystemEnable>(entity);
            if (authoring.HackInputSystemEnable) AddComponent<HackInputSystemEnable>(entity);
            if (authoring.HackSystemEnable) AddComponent<HackSystemEnable>(entity);
            if (authoring.ApplyModifySystemEnable) AddComponent<ApplyModifySystemEnable>(entity);
            if (authoring.SkillCoolDownSystemEnable) AddComponent<SkillCoolDownSystemEnable>(entity);

            if (authoring.DealDamageSystemEnable) AddComponent<DealDamageSystemEnable>(entity);
            if (authoring.DamageSystemEnable) AddComponent<DamageSystemEnable>(entity);
            if (authoring.DeadDestroySystemEnable) AddComponent<DeadDestroySystemEnable>(entity);
            if (authoring.PlayerProjectileSystemEnable) AddComponent<PlayerProjectileSystemEnable>(entity);
            if (authoring.TargetToEnemySystemEnable) AddComponent<TargetToEnemySystemEnable>(entity);
        }
    }
}
public struct SpawnSimpleSystemEnable : IComponentData
{
}
public struct PlayerInputSystemEnable : IComponentData
{
}
public struct PlayerMovementSystemEnable : IComponentData
{
}
public struct EnemyAISystemEnable : IComponentData
{
}

public struct HackInputSystemEnable : IComponentData
{
}
public struct HackSystemEnable : IComponentData
{
}
public struct ApplyModifySystemEnable : IComponentData
{
}
public struct DamageSystemEnable : IComponentData
{
}
public struct DeadDestroySystemEnable : IComponentData
{
}
public struct SkillCoolDownSystemEnable : IComponentData
{
}

public struct DealDamageSystemEnable : IComponentData
{
}
public struct PlayerProjectileSystemEnable : IComponentData
{
}
public struct TargetToEnemySystemEnable : IComponentData
{
}
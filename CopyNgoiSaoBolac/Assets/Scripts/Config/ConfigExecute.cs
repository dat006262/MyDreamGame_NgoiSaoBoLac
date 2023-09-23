
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
    public bool AutoHitSystemEnable;
    public bool SkillAutoTargetSystemEnable;
    public bool ExecuteTriggerSystemEnable;
    public bool DealDamageSystem2Enable;
    public bool E_MorganaSystemEnable;
    public bool W_CamileSystemEnable;
    public bool Q_TemmoSystemEnable;
    public bool SpawnLineSkillSystemEnble;
    public bool Q_MundoSystemEnable;

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
            if (authoring.AutoHitSystemEnable) AddComponent<AutoHitSystemEnable>(entity);
            if (authoring.SkillAutoTargetSystemEnable) AddComponent<SkillAutoTargetSystemEnable>(entity);
            if (authoring.ExecuteTriggerSystemEnable) AddComponent<ExecuteTriggerSystemEnable>(entity);
            if (authoring.DealDamageSystem2Enable) AddComponent<DealDamageSystem2Enable>(entity);
            if (authoring.E_MorganaSystemEnable) AddComponent<E_MorganaSystemEnable>(entity);
            if (authoring.W_CamileSystemEnable) AddComponent<W_CamileSystemEnable>(entity);
            if (authoring.Q_TemmoSystemEnable) AddComponent<Q_TemmoSystemEnable>(entity);
            if (authoring.SpawnLineSkillSystemEnble) AddComponent<SpawnLineSkillSystemEnble>(entity);
            if (authoring.Q_MundoSystemEnable) AddComponent<Q_MundoSystemEnable>(entity);
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
public struct AutoHitSystemEnable : IComponentData
{
}
public struct SpawnLineSkillSystemEnble : IComponentData
{
}
public struct SkillAutoTargetSystemEnable : IComponentData
{
}
public struct ExecuteTriggerSystemEnable : IComponentData
{
}
public struct DealDamageSystem2Enable : IComponentData
{
}
public struct E_MorganaSystemEnable : IComponentData
{
}
public struct W_CamileSystemEnable : IComponentData
{
}
public struct Q_TemmoSystemEnable : IComponentData
{
}
public struct Q_MundoSystemEnable : IComponentData
{
}

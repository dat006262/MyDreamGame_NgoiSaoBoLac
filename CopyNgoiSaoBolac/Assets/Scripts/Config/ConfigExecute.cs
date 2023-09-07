
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
    public bool HackStatusSystemEnable;
    public bool HackSystemEnable;
    public bool ApplyModifySystemEnable;
    public bool SkillCoolDownSystemEnable;
    public bool PositionalWarpingSystemEnable;
    public bool DamageSystemEnable;
    public bool HealthSystemEnable;
    public bool PickupEquippingSystemEnable;
    public bool PickupsSpawnerSystemEnable;
    public bool NPCSpawnerSystemEnable;

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
            if (authoring.HackStatusSystemEnable) AddComponent<HackStatusSystemEnable>(entity);
            if (authoring.HackSystemEnable) AddComponent<HackSystemEnable>(entity);
            if (authoring.ApplyModifySystemEnable) AddComponent<ApplyModifySystemEnable>(entity);
            if (authoring.SkillCoolDownSystemEnable) AddComponent<SkillCoolDownSystemEnable>(entity);
            if (authoring.PositionalWarpingSystemEnable) AddComponent<PositionalWarpingSystemEnable>(entity);
            if (authoring.DamageSystemEnable) AddComponent<DamageSystemEnable>(entity);
            if (authoring.HealthSystemEnable) AddComponent<HealthSystemEnable>(entity);
            if (authoring.PickupEquippingSystemEnable) AddComponent<PickupEquippingSystemEnable>(entity);
            if (authoring.PickupsSpawnerSystemEnable) AddComponent<PickupsSpawnerSystemEnable>(entity);
            if (authoring.NPCSpawnerSystemEnable) AddComponent<NPCSpawnerSystemEnable>(entity);

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

public struct HackStatusSystemEnable : IComponentData
{
}
public struct HackSystemEnable : IComponentData
{
}
public struct ApplyModifySystemEnable : IComponentData
{
}
public struct PlayerProjectileSystemEnable : IComponentData
{
}

public struct PositionalWarpingSystemEnable : IComponentData
{
}
public struct DamageSystemEnable : IComponentData
{
}
public struct HealthSystemEnable : IComponentData
{
}
public struct PickupEquippingSystemEnable : IComponentData
{
}
public struct PickupsSpawnerSystemEnable : IComponentData
{
}
public struct NPCSpawnerSystemEnable : IComponentData
{
}
public struct SkillCoolDownSystemEnable : IComponentData
{
}

using System;
using System.Windows.Input;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class AllComponentData
{

}
//-------------------------------------------------------------------
#region SimpleSpawner
public struct SimpleSpawner_OwnerComponent : IComponentData
{
    // these could be made into an array or something, to spawn multiple types of prefabs
    // keep this pretty generic
    public uint spawnNumber;
}
public struct SimpleSpawner_PrefabAndParentBufferComponent : IBufferElementData
{
    public Entity prefab;
    public Entity parent;
}
#endregion
//----------------------------------------------------------------------------
#region PlayerInputSys
public struct PlayerInput_OwnerComponent : IComponentData
{
    public struct InputPair
    {
        public KeyCode keyCode;
        public bool keyVal;
    }
    public InputPair Up;
    public InputPair Down;
    public InputPair Left;
    public InputPair Right;
    public InputPair Shoot;
    public InputPair Teleport;
    //public InputPair Boost;

}

#endregion
//------------------------------------------------------------------
#region PlayerMoveSys
public struct PlayerMove_OwnerSystem : IComponentData
{
    public float moveSpeed;
    public float rotateSpeed;
}
#endregion
//---------------------------------------------------
#region EnemyAISys
public struct EnemyAI_OwnerComponent : IComponentData
{
    public float moveSpeed;
    public float rotateSpeed;

    ////
    [Header("AI")]
    public float maxChaseDist;
    public float minChaseDist;
}
public struct EnemyStateComponent : IComponentData
{
    public enum State
    {
        FindingTarget = 0,
        AtackTarget = 1,
        StopAtack = 2,
    }
    public State state;
}
#endregion
//---------------------------------Status--------------------------------------
public enum StatType
{
    Strength = 1,
    Health = 2,
    Mana = 3
}
public struct CharacterStat : IComponentData
{

    public float BaseValueStrength;
    public float _strengValue;
    public float BaseValueHealth;
    public float _healthValue;
    public float BaseValueMana;
    public float _manaValue;
}
public enum StatModType
{
    Flat = 100,//cong
    PercentAdd = 200,//VD:value *(1+PercentAdd+PercentAdd)
    PercentMulti = 300//Vd value*(1+PercentMul)*(1+PercentMove)
}
[InternalBufferCapacity(8)]
public struct StatModify : IBufferElementData
{


    public float Value;
    public StatType statType;
    public StatModType statModType;
    public int order;
    public Entity Source;
}
public struct HackInputComponent : IComponentData
{
    public struct InputPair
    {
        public KeyCode keyCode;
        public bool keyVal;
    }
    public InputPair PlusHealth;
    public InputPair PlusMana;
    public InputPair MultiHealth;
    public InputPair Calculate;
    public InputPair UseSkill;
    public InputPair EquipItem;
    public InputPair ChosseItem;
    public InputPair DealDamage;



}
public struct CheckNeedCalculate : IComponentData
{
    public bool dirty;//need?
}

public struct ItemComponent : IComponentData
{
    public int ID;
    public Entity prefab;

}

public struct OwnerByPlayerComponent : IComponentData
{
    public bool owner;

}
public struct EquipByPlayerComponent : IComponentData
{
    public bool equip;
}

//------------SkillCo-----------------------
public enum SkillType
{
    HitEffect = 100,
    Buff = 200,
    Teleport = 300,
    Fire = 400

}
public enum SkillTarget
{
    OneTarget,
    MultiTarget
}
public enum SkillShapeType
{
    Line,
    Round,
    Other
}
public enum SkillDame
{
    OneTime,
    TimeByTime
}
public struct SkillComponent : IComponentData
{
}
public struct SkillCoolDownComponent : IComponentData
{
    public float coolDown;
    public float remain;
    public bool canUse => remain <= 0f;
}
public struct SkillInforComponent : IComponentData
{

    public float damage;
    public float effectFrequenc;
}
public struct SkillEffectComponent : IComponentData
{
    public float effectCountDown;
    public bool canDamage => effectCountDown <= 0f;
}
//--------------Inventory---------------
public struct InventoryComponent : IComponentData
{


}
//-----------------Chosse-------------//
public struct ChosseItemComponent : IComponentData
{
    public int ID;
    public Entity item;
}
//------------------DamageUI----------------------


public struct CharacterAttackStrength : IComponentData//Strength
{
    public float Value;
}

public struct CharacterExperiencePoints : IComponentData//EXP
{
    public float Value;
}

public struct CharacterHitPoints : IComponentData//Health
{
    public float Value;
}

public struct DamageToCharacter : IComponentData//DamageFrom?
{
    public float Value;
    public Entity OriginCharacter;
    public float effectFrequenc;
    public float effectCount;
    public bool CandealDamage => effectCount <= 0;
}
//-----------------------------DeadDestroyTag----------------------
public struct DeadDestroyTag : IComponentData//Health
{

}
//------------------------------Fire----------------------------
public struct EquippedProjectileDataComponent : IComponentData, IEnableableComponent
{
    public bool active;
    public Entity owner;
    public Entity activeVisual;
    public Entity prefab;
    public double timeToLive;
    public float speed;
    public float scale;
    public double pickupTime;
    public double pickupTimeToLive;
    public bool isCollisionInvulnerable;
}

public struct PickupProjectileDataComponent : IComponentData
{
    public bool active;
    public Entity activeVisual;
    public Entity prefab;
    public float timeToLive;
    public float speed;
    public float scale;
    //public double pickupTime;
    public double pickupTimeToLive;
    public bool isCollisionInvulnerable;
}
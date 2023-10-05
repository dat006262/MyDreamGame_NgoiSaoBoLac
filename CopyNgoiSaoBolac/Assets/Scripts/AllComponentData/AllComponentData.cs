using System;
using System.Windows.Input;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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

public struct EnemyAISys_TargetComponent : IComponentData { }
#endregion
//-------------------------------------
#region TargetToEnemySystem
public struct TargetToEnemySy_OwnerComponent : IComponentData
{
    public float turnSpeed;

}
public struct TargetToEnemySy_TargetComponent : IComponentData { }
#endregion
//-----------------------------------------------------------------------
#region Status
public enum StatType
{
    Strength = 1,
    Health = 2,
    Mana = 3
}
public struct CharacterStat : IComponentData
{

    public float BaseValueStrength;
    public float _strengMaxValue;
    public float BaseValueHealth;
    public float _healthMaxValue;
    public float BaseValueMana;
    public float _manaMaxValue;
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
#endregion
//-------------------------------------------------------------------------------
#region HackSYS +CalculateStat
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
public struct CharacterAttackStrength : IComponentData//Strength
{
    public float Value;
}
public struct ChosseItemComponent : IComponentData
{
    public int ID;
    public Entity item;
}



public struct OwnerByPlayerComponent : IComponentData
{
    public bool owner;

}
public struct EquipByPlayerComponent : IComponentData
{
    public bool equip;
}
#endregion
//------------SkillCo-----------------------



//-----------------------------------------
#region SkillCoolDownSys
public struct SkillCoolDownSys_OwnerComponent : IComponentData
{
    public float coolDown;
    public float remain;
    public bool canUse => remain <= 0f;
}
#endregion 
//----------------------------------
#region DamageSys
public struct DamageSys_OwnerComponent : IComponentData//gan len skill
{

    public float damage;
    public float effectFrequenc;
}
public struct DamageSys_HealthComponent : IComponentData//Health
{
    public float Value;
}
#endregion
//-------------------------------------------
#region DealDamageSys
public struct DealDamageSys_EffectCountDownComponent : IComponentData
{
    public float effectCountDown;
    public bool canDamage => effectCountDown <= 0f;
}
public struct DealDamageSys_EXPComponent : IComponentData//EXP
{
    public float Value;
}
public struct DealDamageSys_OwnerComponent : IComponentData//DamageFrom?
{
    public float Value;
    public Entity OriginCharacter;
    public float effectFrequenc;
    public float effectCount;
    public bool CandealDamage => effectCount <= 0;
}
#endregion

//---------------------------------------------------
#region DeadSys
public struct DeadDestroyTag : IComponentData//Health
{
    public float DeadAfter;
}
#endregion
//----------------------------------------------------------

#region PlayerProjecTileSys
public struct PlayerProjecTileSys_OwnerComponent : IComponentData
{

}
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
#endregion
//================================================

//--------------------------------------------------
#region AutoHitSys//SpawnAutoHit
public struct AutoHitSys_PrefabComponent : IComponentData
{


}
public struct AutoHitSys_OwnerComponent : IComponentData
{
    public bool active;
    public Entity prefab;
    public float timeToLive;

}
#endregion
//Chung ta can 1 Button; code cho button; Send Data to Config; SpawnSystem
//-------------------------------------
#region SkillAutoTargetSys
public struct SkillAutoTargetSys_OwnerComponent : IComponentData
{
    public bool active;
    public Entity prefab;
    public float RangeSkill;


}
public struct SkillAutoTargetSys_TargetComponent : IComponentData { }

#endregion

//-----------------------------
#region SpawnLineSkillSys
public struct SpawnLineSkillSys_OwnerComponent : IComponentData
{
    public bool active;
    public Entity prefab;
    public float timeToLive;
}

#endregion
//----------------------------------------------
#region ExecuteTriggerSys
public struct ExecuteTriggerSys_OwnerComponent : IComponentData//gan len skill
{
    public bool active;
    public SkillType type;
    public float damage;
    public float effectFrequenc;
}
public struct ExecuteTriggerSys_HealthComponent : IComponentData//Health
{
    public float Value;
}
#endregion
//------------------------------------------------
#region DealDamageSys2
public struct DealDamageSys2_OwnerComponent : IBufferElementData//DamageFrom?
{
    public float effectCount;
    public bool CandealDamage => effectCount <= 0;
    public bool CanRemove => loopCount <= 0;

    //-----
    public bool isLoop; //co lap lai dame ko
    public int loopCount;//so lan lap lai
    public float Value;//damage
    public Entity OriginCharacter;
    public float effectFrequenc;//thoi gian lap lai
    public SkillType type;


}
#endregion
//===============================================

//------------------------
#region SkillInfor
public struct SkillComponent : IComponentData
{
}
public enum SkillType
{
    Q_Temmo,
    Q_Mundo,
    Q_Zilein,
    E_Morgana,
    W_Camile
}
public struct E_MorganaEffectTag : IComponentData
{
}
public struct E_MorganaComponent : IComponentData
{
    public bool active;
    public float countdown;
    public int loopCount;
    public float BasicDamePerloop;
    public Entity OriginCharacter;
    public float effectFrequenc;
}
public struct W_CamileEffectTag : IComponentData
{

}
public struct W_CamileComponent : IComponentData
{
    public bool active;
    public float damageAfter;//2sneu con trong tam hoat dong
    public Entity OriginCharacter;
    public float DamageBasic;
}
public struct Q_TemmoSaveTargetComponent : IComponentData
{
    public Entity TargetTo;
    public float3 TargetPos;
}
public struct Q_TemmoComponent : IComponentData
{
    public bool active;
    public float DamageBasic;
    public float flySpeed;
    public Entity OriginCharacter;
    public float distancesDealDamage;


}
public struct Q_MundoEffectTag : IComponentData
{
}
public struct Q_MundoComponent : IComponentData
{
    public bool active;
    public float flySpeed;
    public Entity OriginCharacter;
    public float DamageBasic;
}
public struct Q_ZileinEffectTag : IComponentData
{
}
public struct Q_ZileinComponent : IComponentData
{
}
#endregion
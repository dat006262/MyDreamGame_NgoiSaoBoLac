using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SkillAuthoring : MonoBehaviour
{
    public float SkillDamage;
    public float effectFrequenc = 1f;
    public SkillType type;
    public class SkillBaker : Baker<SkillAuthoring>
    {

        public override void Bake(SkillAuthoring authoring)
        {
            var ent = GetEntity(authoring);
            ////How to Equip //UnEquip //
            AddComponent<SkillComponent>();
            AddComponent<SkillCoolDownSys_OwnerComponent>(new SkillCoolDownSys_OwnerComponent

            {
                coolDown = 10,
                remain = 0
            });
            AddComponent<DamageSys_OwnerComponent>(
                new DamageSys_OwnerComponent
                {
                    damage = authoring.SkillDamage,
                    effectFrequenc = authoring.effectFrequenc
                }
                );


            AddComponent<ExecuteTriggerSys_OwnerComponent>(
             new ExecuteTriggerSys_OwnerComponent
             {
                 active = true,
                 type = authoring.type,
                 damage = authoring.SkillDamage,
                 effectFrequenc = authoring.effectFrequenc
             }
             );

        }
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SkillAuthoring : MonoBehaviour
{
    public float SkillDamage;
    public float effectFrequenc = 1f;
    public class SkillBaker : Baker<SkillAuthoring>
    {

        public override void Bake(SkillAuthoring authoring)
        {
            var ent = GetEntity(authoring);
            ////How to Equip //UnEquip //
            AddComponent<SkillComponent>();
            AddComponent<SkillCoolDownComponent>(new SkillCoolDownComponent

            {
                coolDown = 10,
                remain = 0
            });
            AddComponent<SkillInforComponent>(
                new SkillInforComponent
                {
                    damage = authoring.SkillDamage,
                    effectFrequenc = authoring.effectFrequenc
                }
                );

        }
    }

}

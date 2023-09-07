using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SkillAuthoring : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
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
                    damage = 10
                }
                );

        }
    }

}

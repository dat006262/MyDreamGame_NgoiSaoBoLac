using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Q_TemmoAuthoring : MonoBehaviour
{
    public float flySpeed = 100;
    public float distancesDealDamage = 0.5f;
    public CowdControl[] cowdControls;
    public class Q_TemmoBaker : Baker<Q_TemmoAuthoring>
    {

        public override void Bake(Q_TemmoAuthoring authoring)
        {
            AddComponent<Q_TemmoComponent>(
                new Q_TemmoComponent
                {
                    active = true,
                    flySpeed = authoring.flySpeed,
                    DamageBasic = 80,
                    OriginCharacter = Entity.Null,
                    distancesDealDamage = authoring.distancesDealDamage
                });
            AddComponent<Q_TemmoSaveTargetComponent>();

            var buffer = AddBuffer<DealHardControl_Component>();
            for (int i = 0; i < authoring.cowdControls.Length; i++)
            {
                //DependsOn(authoring.cowdControls[i]);
                buffer.Add(new DealHardControl_Component
                {
                    type = authoring.cowdControls[i].type,
                    time = authoring.cowdControls[i].time,
                    sloweffect = authoring.cowdControls[i].sloweffect
                });
            }
        }
    }
}

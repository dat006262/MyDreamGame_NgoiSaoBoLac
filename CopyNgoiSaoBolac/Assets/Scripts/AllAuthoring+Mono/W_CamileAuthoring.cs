using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class W_CamileAuthoring : MonoBehaviour
{
    public float damageAfter = 2;
    public float damageBasic = 200;
    public class W_CamileBaker : Baker<W_CamileAuthoring>
    {

        public override void Bake(W_CamileAuthoring authoring)
        {
            AddComponent<W_CamileComponent>(
                new W_CamileComponent
                {
                    damageAfter = authoring.damageAfter,
                    DamageBasic = authoring.damageBasic,
                    OriginCharacter = Entity.Null
                });
            ;
        }
    }
}

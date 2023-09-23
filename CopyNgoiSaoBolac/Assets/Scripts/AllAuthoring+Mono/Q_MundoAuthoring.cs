using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Q_MundoAuthoring : MonoBehaviour
{
    public float DamageBasic = 100;
    public float flySpeed = 10;
    public class Q_MundoBaker : Baker<Q_MundoAuthoring>
    {

        public override void Bake(Q_MundoAuthoring authoring)
        {
            AddComponent<Q_MundoComponent>(
                new Q_MundoComponent
                {
                    DamageBasic = authoring.DamageBasic,
                    flySpeed = authoring.flySpeed,
                    OriginCharacter = Entity.Null
                });
        }
    }
}

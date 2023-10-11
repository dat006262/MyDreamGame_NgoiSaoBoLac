using System.Collections;
using Unity.Entities;
using UnityEngine;


public class E_MorganaAuthoring : MonoBehaviour
{

    public class E_MorganaBaker : Baker<E_MorganaAuthoring>
    {

        public override void Bake(E_MorganaAuthoring authoring)
        {
            AddComponent<E_MorganaComponent>(
                new E_MorganaComponent
                {
                    active = true,
                    BasicDamePerTurn = 10,
                    countdown = 0.1f,
                    effectFrequenc = 0.6f,
                    turnCount = 5,
                    OriginCharacter = Entity.Null
                });
        }
    }
}

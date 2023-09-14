using System.Collections;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.AllAuthoring_Mono
{
    public class TargetToEnemySys_OwnerAuthoring : MonoBehaviour
    {
        public int turnSpeed = 10;
        public class TargetToEnemySys_OwnerBaker : Baker<TargetToEnemySys_OwnerAuthoring>
        {

            public override void Bake(TargetToEnemySys_OwnerAuthoring authoring)
            {
                AddComponent<TargetToEnemySy_OwnerComponent>(new TargetToEnemySy_OwnerComponent { turnSpeed = authoring.turnSpeed });
            }
        }
    }
}
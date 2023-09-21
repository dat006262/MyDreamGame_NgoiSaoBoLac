using System.Collections;
using Unity.Entities;
using UnityEngine;


public class AutoHitSys_OwnerAuthoring : MonoBehaviour
{
    public bool active = true;
    public GameObject prefab;
    public class AutoHitSys_OwnerBaker : Baker<AutoHitSys_OwnerAuthoring>
    {

        public override void Bake(AutoHitSys_OwnerAuthoring authoring)
        {
            AddComponent<AutoHitSys_OwnerComponent>(new AutoHitSys_OwnerComponent
            {
                active = authoring.active,
                prefab = GetEntity(authoring.prefab),
                timeToLive = 0,
            }
            );
        }
    }
}

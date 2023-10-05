using System.Collections;
using Unity.Entities;
using UnityEngine;


public class W_CamileSpawnSys_OwnerAuthoring : MonoBehaviour
{
    public bool active = true;
    public GameObject prefab;
    public class AutoHitSys_OwnerBaker : Baker<W_CamileSpawnSys_OwnerAuthoring>
    {

        public override void Bake(W_CamileSpawnSys_OwnerAuthoring authoring)
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

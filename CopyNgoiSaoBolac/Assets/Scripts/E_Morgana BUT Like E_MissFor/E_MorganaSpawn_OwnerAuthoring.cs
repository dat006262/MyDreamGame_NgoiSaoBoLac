using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class E_MorganaSpawn_OwnerAuthoring : MonoBehaviour
{
    public bool active = true;
    public GameObject prefab;

    public class E_MorganaSpawn_OwnerBaker : Baker<E_MorganaSpawn_OwnerAuthoring>
    {

        public override void Bake(E_MorganaSpawn_OwnerAuthoring authoring)
        {
            AddComponent<E_MorganaSpawn_OwnerComponent>(new E_MorganaSpawn_OwnerComponent
            {
                active = authoring.active,
                prefab = GetEntity(authoring.prefab)
            }
            );
        }
    }
}
public struct E_MorganaSpawn_OwnerComponent : IComponentData
{
    public bool active;
    public Entity prefab;

}
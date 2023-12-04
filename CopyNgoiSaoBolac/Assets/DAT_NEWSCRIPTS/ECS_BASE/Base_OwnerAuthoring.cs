using DAT;
using Unity.Entities;
using UnityEngine;
public class Base_OwnerAuthoring : MonoBehaviour
{
    //Bien sinh ra de =o day
    public class Base_OwnerBaker : Baker<Base_OwnerAuthoring>
    {
        public override void Bake(Base_OwnerAuthoring authoring)
        {
            AddComponent<Base_Tag>();
        }
    }
}
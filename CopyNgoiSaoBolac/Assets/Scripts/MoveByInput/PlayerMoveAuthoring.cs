
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerMoveAuthoring : MonoBehaviour
{
    public float moveSpeed = 1;
    public float rotateSpeed = 1;

    public class PlayerBaker : Baker<PlayerMoveAuthoring>
    {
        public override void Bake(PlayerMoveAuthoring authoring)
        {
            // var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerMove_OwnerComponent>(/*entity, */new PlayerMove_OwnerComponent
            {
                active = true,
                moveSpeed = authoring.moveSpeed,
                rotateSpeed = authoring.rotateSpeed
            });

        }
    }
}

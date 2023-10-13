
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerMoveAuthoring : MonoBehaviour
{
    public bool isMove;

    public class PlayerBaker : Baker<PlayerMoveAuthoring>
    {
        public override void Bake(PlayerMoveAuthoring authoring)
        {
            // var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerMove_OwnerComponent>(/*entity, */new PlayerMove_OwnerComponent
            {
                active = true,
                isMove = false,
                isRight = true
            });

        }
    }
}
public struct PlayerMove_OwnerComponent : IComponentData
{
    public bool active;
    public bool isMove;
    public bool isRight;
}

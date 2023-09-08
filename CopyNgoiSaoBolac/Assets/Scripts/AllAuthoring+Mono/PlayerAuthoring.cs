
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float moveSpeed = 1;
    public float rotateSpeed = 1;

    public class PlayerBaker : Baker<PlayerAuthoring>
    {

        public override void Bake(PlayerAuthoring authoring)
        {
            // var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerComponent>(/*entity, */new PlayerComponent
            {
                moveSpeed = authoring.moveSpeed,
                rotateSpeed = authoring.rotateSpeed
            });

            AddComponent<ChosseItemComponent>(new ChosseItemComponent { ID = 0, item = Entity.Null });

        }
    }
}


using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyAuthoring : MonoBehaviour
{
    public float moveSpeed = 1;
    public float rotateSpeed = 1;
    /// <summary>
    /// 
    /// </summary>

    public float minChaseDist = 2f;



    public class EnemyBaker : Baker<EnemyAuthoring>
    {

        public override void Bake(EnemyAuthoring authoring)
        {
            var entity = GetEntity();
            AddComponent<EnemyAI_OwnerComponent>(/*entity, */new EnemyAI_OwnerComponent
            {
                moveSpeed = authoring.moveSpeed,
                rotateSpeed = authoring.rotateSpeed,
                ///

                minChaseDist = authoring.minChaseDist
            });


        }
    }
}
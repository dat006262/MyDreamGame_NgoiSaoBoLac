
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
    public float maxChaseDist = 7;
    public float minChaseDist = 0.05f;

    public int StartingHitPoints;
    public int ExperiencePointsValue;

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
                maxChaseDist = authoring.maxChaseDist,
                minChaseDist = authoring.minChaseDist
            });


            AddComponent(new DamageSys_HealthComponent { Value = authoring.StartingHitPoints });
            AddComponent(new DealDamageSys_EXPComponent { Value = authoring.ExperiencePointsValue });

            AddComponent(new DealDamageSys_OwnerComponent
            {
                Value = 0,
                OriginCharacter = entity
            });


            AddComponent(new ExecuteTriggerSys_HealthComponent { Value = authoring.StartingHitPoints });

            AddBuffer<DealDamageSys2_OwnerComponent>();
        }
    }
}
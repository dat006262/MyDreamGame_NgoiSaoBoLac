
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
            AddComponent<EnemyComponent>(/*entity, */new EnemyComponent
            {
                moveSpeed = authoring.moveSpeed,
                rotateSpeed = authoring.rotateSpeed,
                ///
                maxChaseDist = authoring.maxChaseDist,
                minChaseDist = authoring.minChaseDist
            });


            AddComponent(new CharacterHitPoints { Value = authoring.StartingHitPoints });
            AddComponent(new CharacterExperiencePoints { Value = authoring.ExperiencePointsValue });

            AddComponent(new DamageToCharacter
            {
                Value = 0,
                OriginCharacter = entity
            });

        }
    }
}
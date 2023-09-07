
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    public float moveSpeed = 1;
    public float rotateSpeed = 1;
    /// <summary>
    /// 
    /// </summary>
    public float maxChaseDist = 7;
    public float minChaseDist = 0.05f;

    public class EnemyBaker : Baker<EnemyAuthoring>
    {

        public override void Bake(EnemyAuthoring authoring)
        {

            AddComponent<EnemyComponent>(/*entity, */new EnemyComponent
            {
                moveSpeed = authoring.moveSpeed,
                rotateSpeed = authoring.rotateSpeed,
                ///
                maxChaseDist = authoring.maxChaseDist,
                minChaseDist = authoring.minChaseDist
            });

        }
    }
}
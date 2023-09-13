using System.Collections;
using Unity.Entities;
using UnityEngine;


public class EnemyAI_TargerAuthorng : MonoBehaviour
{
    public class EnemyAI_TargetBaker : Baker<EnemyAI_TargerAuthorng>
    {

        public override void Bake(EnemyAI_TargerAuthorng authoring)
        {
            AddComponent<EnemyAISys_TargetComponent>();
        }
    }
}
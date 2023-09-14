using System.Collections;
using Unity.Entities;
using UnityEngine;

public class TargetToEnemySys_TargetAuthoring : MonoBehaviour
{

    public class TargetToEnemySys_Targetbaker : Baker<TargetToEnemySys_TargetAuthoring>
    {

        public override void Bake(TargetToEnemySys_TargetAuthoring authoring)
        {
            AddComponent<TargetToEnemySy_TargetComponent>();
        }
    }
}

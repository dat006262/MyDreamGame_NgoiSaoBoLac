using DAT;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MoveByInput_OwnerAuthoring : MonoBehaviour
{
    public class MoveByInput_OwnerBaker : Baker<MoveByInput_OwnerAuthoring>
    {
        public override void Bake(MoveByInput_OwnerAuthoring authoring)
        {
            AddComponent<MoveByInputTag>();
            AddComponent<MoveByInput_Speed>(new MoveByInput_Speed { speed = 100 });
            AddComponent<MoveByInput_CheckIsMove>(new MoveByInput_CheckIsMove { ismove = false });
            AddComponent<MoveByInput_CheckIsRight>(new MoveByInput_CheckIsRight { isRight = true });
        }
    }
}

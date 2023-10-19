using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace DAT
{
    public struct MoveByInputTag : IComponentData { }
    public struct MoveByInput_Speed : IComponentData
    {
        public float speed;
    }
    public struct MoveByInput_CheckIsMove : IComponentData
    {
        public bool ismove;
    }
    public struct MoveByInput_CheckIsRight : IComponentData
    {
        public bool isRight;
    }
}
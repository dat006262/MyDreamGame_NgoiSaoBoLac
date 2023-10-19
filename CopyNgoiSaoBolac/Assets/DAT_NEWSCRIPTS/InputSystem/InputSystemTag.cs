using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
namespace DAT
{
    public struct InputSystemTag : IComponentData { }
    public struct PlayerInput_WorkComponent : IComponentData
    {
        public struct InputPair
        {
            public KeyCode keyCode;
            public bool keyVal;
        }
        public InputPair Up;
        public InputPair Down;
        public InputPair Left;
        public InputPair Right;

    }

    public struct PlayerInput_OutDataComponent : IComponentData
    {
        public bool Up;
        public bool Down;
        public bool Left;
        public bool Right;
    }
}

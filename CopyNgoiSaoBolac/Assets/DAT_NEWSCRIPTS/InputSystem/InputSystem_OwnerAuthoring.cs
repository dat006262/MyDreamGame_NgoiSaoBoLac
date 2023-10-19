using DAT;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InputSystem_OwnerAuthoring : MonoBehaviour
{
    public KeyCode Up;
    public KeyCode Down;
    public KeyCode Left;
    public KeyCode Right;

    public class InputSystem_OwnerBaker : Baker<InputSystem_OwnerAuthoring>
    {
        public override void Bake(InputSystem_OwnerAuthoring authoring)
        {
            AddComponent<InputSystemTag>();
            AddComponent<PlayerInput_OutDataComponent>();
            AddComponent<PlayerInput_WorkComponent>(new PlayerInput_WorkComponent
            {
                Up = new PlayerInput_WorkComponent.InputPair { keyCode = authoring.Up },
                Down = new PlayerInput_WorkComponent.InputPair { keyCode = authoring.Down },
                Left = new PlayerInput_WorkComponent.InputPair { keyCode = authoring.Left },
                Right = new PlayerInput_WorkComponent.InputPair { keyCode = authoring.Right }
            });
        }
    }
}

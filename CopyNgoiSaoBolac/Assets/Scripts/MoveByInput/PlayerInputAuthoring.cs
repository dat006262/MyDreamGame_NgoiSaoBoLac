using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public class PlayerInputAuthoring : MonoBehaviour
{
    public KeyCode Up;
    public KeyCode Down;
    public KeyCode Left;
    public KeyCode Right;
    public KeyCode Shoot;
    public KeyCode Teleport;

    public KeyCode PlusHealth;
    public KeyCode PlusMana;
    public KeyCode MultiHealth;
    public KeyCode Calculate;
    public KeyCode EquipItem;
    public KeyCode UseSkill;

    public KeyCode ChosseItem;
    public KeyCode DealDamage;
    public class PlayerBaker : Baker<PlayerInputAuthoring>
    {
        public override void Bake(PlayerInputAuthoring authoring)
        {
            AddComponent<PlayerInput_OwnerComponent>(new PlayerInput_OwnerComponent
            {
                Up = new PlayerInput_OwnerComponent.InputPair { keyCode = authoring.Up },
                Down = new PlayerInput_OwnerComponent.InputPair { keyCode = authoring.Down },
                Left = new PlayerInput_OwnerComponent.InputPair { keyCode = authoring.Left },
                Right = new PlayerInput_OwnerComponent.InputPair { keyCode = authoring.Right },
                Shoot = new PlayerInput_OwnerComponent.InputPair { keyCode = authoring.Shoot },
                Teleport = new PlayerInput_OwnerComponent.InputPair { keyCode = authoring.Teleport }
            });

            //AddComponent<HackInputComponent>(new HackInputComponent
            //{
            //    PlusHealth = new HackInputComponent.InputPair { keyCode = authoring.PlusHealth },
            //    PlusMana = new HackInputComponent.InputPair { keyCode = authoring.PlusMana },
            //    MultiHealth = new HackInputComponent.InputPair { keyCode = authoring.MultiHealth },
            //    Calculate = new HackInputComponent.InputPair { keyCode = authoring.Calculate },
            //    EquipItem = new HackInputComponent.InputPair { keyCode = authoring.EquipItem },
            //    UseSkill = new HackInputComponent.InputPair { keyCode = authoring.UseSkill },
            //    ChosseItem = new HackInputComponent.InputPair { keyCode = authoring.ChosseItem },
            //    DealDamage = new HackInputComponent.InputPair { keyCode = authoring.DealDamage },
            //});
        }
    }
}
